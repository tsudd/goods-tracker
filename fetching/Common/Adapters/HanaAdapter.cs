using Sap.Data.Hana;
using Common.Trackers;
using Microsoft.Extensions.Logging;
using Models;
using System.Reflection;
using Common.Configs;

namespace Common.Adapters
{
    public class HanaAdapter : IDataAdapter
    {
        public const string CREATE_STREAM_COMMAND =
            "INSERT INTO STREAM VALUES (STREAMSEQID.nextval, CURRENT_TIMESTAMP(0))";
        private ILogger _logger;
        private AdapterConfig _config;

        public HanaAdapter(AdapterConfig config, ILogger<HanaAdapter> logger)
        {
            _logger = logger;
            _config = config;
        }

        public void SaveItems(ITracker tracker, IEnumerable<string> shopIds)
        {
            try
            {
                using (var conn = new HanaConnection(_config.Arguments))
                {
                    conn.Open();

                    using (var cmd = new HanaCommand(CREATE_STREAM_COMMAND, conn))
                    {
                        cmd.ExecuteNonQuery();

                        foreach (var shop in shopIds)
                        {
                            cmd.Parameters.Clear();
                            var items = tracker.GetShopItems(shop);
                            if (items is null || items.Count() == 0)
                            {
                                _logger.LogWarning($"No items to save for {shop}");
                                continue;
                            }
                            cmd.CommandText = GenerateSaveCommand(shop);

                            FillBatchArguments(cmd.Parameters, items);
                            try
                            {
                                var rowsAffected = cmd.ExecuteNonQuery();
                                _logger.LogInformation($"Saved {rowsAffected} items from {shop} shop");
                            }
                            catch (Exception ex)
                            {
                                _logger.LogWarning($"Couldn't save items to the data: {ex.Message}");
                                throw new AdapterException(ex.Message, shop);
                            }
                        }
                    }
                }
            }
            catch (HanaException ex)
            {
                _logger.LogError($"Error within communication with HANA: {ex.Message}");
                throw new ApplicationException(ex.Message);
            }
            catch (AdapterException ex)
            {
                _logger.LogError($"Error during items save from {ex.ShopId}: {ex.Message}");
                throw new ApplicationException(ex.Message, ex);
            }
        }

        private string GenerateSaveCommand(string shopId)
        => $"INSERT INTO ITEM VALUES(ITEMSEQID.NEXTVAL,?,?,?,?,?,?,'{shopId}',STREAMSEQID.CURRVAL)";

        private void FillBatchArguments(HanaParameterCollection paramsCollection, IEnumerable<Item> fetchedItems)
        {
            foreach (var item in fetchedItems)
            {
                var paramInd = 0;
                foreach (var props in typeof(Item).GetProperties())
                {
                    paramsCollection.Add(CreateParameter("p" + paramInd, props.GetValue(item)));
                    paramInd++;
                }
            }
        }

        private HanaParameter CreateParameter(string pName, object? paramValue)
        {
            var param = new HanaParameter(pName, HanaDbType.VarChar);
            param.Value = paramValue;
            return param;
        }
    }
}