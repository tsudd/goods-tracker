using Common.Configs;
using Common.Trackers;
using CsvHelper;
using Microsoft.Extensions.Logging;
using Models;
using System.Globalization;

namespace Common.Adapters
{
    public class CsvAdapter : IDataAdapter
    {
        private ILogger _logger;
        private AdapterConfig _config;
        public CsvAdapter(AdapterConfig config, ILogger<CsvAdapter> logger)
        {
            _config = config;
            _logger = logger;
        }
        public void SaveItems(ITracker tracker, IEnumerable<string> shopIds)
        {
            _logger.LogInformation("Writing items into CSV files...");
            var time = DateTime.Now.ToString("dd.MM.yyyy_HH-mm-ss");
            foreach (var shop in shopIds)
            {
                var items = tracker.GetShopItems(shop);
                if (items is null || items.Count() == 0)
                {
                    _logger.LogWarning($"No items to save for {shop}");
                    continue;
                }
                try
                {
                    using (var fs = new StreamWriter($"shop_{shop}_{time}.csv"))
                    using (var csvWriter = new CsvWriter(fs, CultureInfo.CurrentCulture))
                    {
                        csvWriter.WriteHeader<Item>();
                        csvWriter.NextRecord();
                        csvWriter.WriteRecords(items);
                        _logger.LogInformation(
                            $"{items.Count()} items from shop '1' were writed to the shop_{shop}_{time}.csv");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Couldn't write shop '{shop}' items to a CSV: {ex.Message}");
                }
            }
        }
    }
}