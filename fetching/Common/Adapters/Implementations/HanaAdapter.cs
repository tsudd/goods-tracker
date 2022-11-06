using Sap.Data.Hana;
using Microsoft.Extensions.Logging;
using GoodsTracker.DataCollector.Models;
using GoodsTracker.DataCollector.Common.Configs;
using GoodsTracker.DataCollector.Common.Trackers.Interfaces;
using GoodsTracker.DataCollector.Common.Adapters.Exceptions;
using GoodsTracker.DataCollector.Common.Adapters.Interfaces;

namespace GoodsTracker.DataCollector.Common.Adapters.Implementations;
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

    public void SaveItems(IItemTracker tracker, IEnumerable<string> shopIds)
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
                        cmd.CommandText =
                            $"SELECT ID, NAME1 FROM ITEM WHERE NAME1 in ({string.Join(",", items.Select(item => "'" + item.Name1 + "'"))})";
                        var existingItems = new List<string>();
                        using (var reader = cmd.ExecuteReader())
                        {
                            cmd.CommandText = GenerateItemRecordSaveCommand();
                            while (reader.Read())
                            {
                                var itemToUpdate = (from item in items
                                                    where item.Name1 == reader[1].ToString()
                                                    select item).Single();
                                cmd.Parameters.Add(CreateParameter("p0", reader[0]));
                                cmd.Parameters.Add(CreateParameter("p1", itemToUpdate.Price));
                                cmd.Parameters.Add(CreateParameter("p2", itemToUpdate.Discount));
                                cmd.Parameters.Add(CreateParameter("p3", IsOnDiscount(itemToUpdate)));
                                existingItems.Add(itemToUpdate.Name1!);
                            }
                        }
                        if (existingItems.Count > 0)
                        {
                            cmd.ExecuteNonQuery();
                        }

                        var newItems = from item in items
                                       where !existingItems.Any(existing => existing == item.Name1)
                                       select item;
                        cmd.CommandText = GenerateItemSaveCommand(shop);
                        using (var subCmd = new HanaCommand(GenerateItemRecordSaveCommand("ITEMSEQID.CURRVAL"), conn))
                        {
                            foreach (var item in newItems)
                            {
                                cmd.Parameters.Clear();
                                subCmd.Parameters.Clear();
                                cmd.Parameters.Add(CreateParameter("p0", item.Name1));
                                cmd.Parameters.Add(CreateParameter("p1", item.Name2));
                                cmd.Parameters.Add(CreateParameter("p2", item.Name3));
                                cmd.Parameters.Add(CreateParameter("p3", item.Link));
                                cmd.Parameters.Add(CreateParameter("p4", item.Country));
                                cmd.Parameters.Add(CreateParameter("p5", item.Producer));
                                cmd.Parameters.Add(CreateParameter("p6", item.VendorCode));
                                cmd.Parameters.Add(CreateParameter("p7", item.Wieght));
                                cmd.Parameters.Add(CreateParameter("p8", item.WieghtUnit));
                                cmd.Parameters.Add(CreateParameter("p9", item.Compound));
                                cmd.Parameters.Add(CreateParameter("p10", item.Protein));
                                cmd.Parameters.Add(CreateParameter("p11", item.Fat));
                                cmd.Parameters.Add(CreateParameter("p12", item.Carbo));
                                cmd.Parameters.Add(CreateParameter("p13", item.Portion));
                                subCmd.Parameters.Add(CreateParameter("p0", item.Price));
                                subCmd.Parameters.Add(CreateParameter("p1", item.Discount));
                                subCmd.Parameters.Add(CreateParameter("p2", IsOnDiscount(item)));
                                cmd.ExecuteNonQuery();
                                subCmd.ExecuteNonQuery();

                                if (item.Categories is not null)
                                {
                                    subCmd.Parameters.Clear();
                                    subCmd.CommandText = $"SELECT ID, NAME FROM CATEGORY WHERE NAME in ({string.Join(",", item.Categories.Select(cat => "'" + cat + "'"))})";
                                    var existingCategoryHashes = new List<int>();
                                    var newCategoryNames = new List<string>();
                                    using (var reader = subCmd.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            existingCategoryHashes.Add(reader.GetInt32(0));
                                            newCategoryNames.Add(reader.GetString(1));
                                        }
                                    }
                                    var categoriesToCreate = from cat in item.Categories
                                                             where !newCategoryNames.Contains(cat)
                                                             select cat;
                                    if (categoriesToCreate.Count() > 0)
                                    {
                                        subCmd.CommandText = $"INSERT INTO CATEGORY VALUES(?, ?)";
                                        foreach (var newCat in categoriesToCreate)
                                        {
                                            subCmd.Parameters.Add(CreateParameter("p0", newCat.GetHashCode()));
                                            subCmd.Parameters.Add(CreateParameter("p1", newCat));
                                            existingCategoryHashes.Add(newCat.GetHashCode());
                                        }
                                        subCmd.ExecuteNonQuery();

                                        subCmd.Parameters.Clear();
                                    }

                                    if (existingCategoryHashes.Count > 0)
                                    {
                                        subCmd.CommandText = $"INSERT INTO CATLINK VALUES(ITEMSEQID.CURRVAL, ?)";
                                        foreach (var catHash in existingCategoryHashes)
                                        {
                                            subCmd.Parameters.Add(CreateParameter("p0", catHash));
                                        }
                                        subCmd.ExecuteNonQuery();
                                    }
                                    subCmd.CommandText = GenerateItemRecordSaveCommand("ITEMSEQID.CURRVAL");
                                }
                            }
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
        catch (Exception ex)
        {
            _logger.LogCritical($"Error within communication with HANA: {ex.Message}");
            throw new ApplicationException(ex.Message);
        }
    }

    private int IsOnDiscount(Item item)
    => item.Discount is not null ? 1 : 0;

    private string GenerateItemSaveCommand(string shopId)
    => $"INSERT INTO ITEM VALUES(ITEMSEQID.NEXTVAL,?,?,?,?,?,?,?,?,?,?,?,?,?,?,'{shopId}')";

    private string GenerateItemRecordSaveCommand(string itemId = "?")
    => $"INSERT INTO ITEMRECORD VALUES(STREAMSEQID.CURRVAL, {itemId}, ?, ?, ?)";

    private void FillBatchArguments(HanaParameterCollection paramsCollection, IEnumerable<Item> fetchedItems)
    {
        foreach (var item in fetchedItems)
        {
            var paramInd = 0;
            foreach (var props in typeof(Item).GetProperties())
            {
                paramsCollection.Add(CreateParameter("p" + paramInd, props.GetValue(item)));
                paramInd++;
                if (paramInd == 5)
                    paramInd = 0;
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
