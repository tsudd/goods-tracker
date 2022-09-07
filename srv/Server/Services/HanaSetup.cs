using Sap.Data.Hana;
using GoodsTracker.Shared.Model;
using Server.DbOptions;
using System.Data.Common;

namespace Server.Services;
public class HanaSetup : IDbSetup
{
    private ILogger _logger;
    private DbContext _dbContext;
    private HanaConnection? _conn;

    public HanaSetup(ILogger<HanaSetup> logger, DbContext context)
    {
        _logger = logger;
        _dbContext = context;
    }

    public async Task<IEnumerable<Item>> GetPaginatedItemsAsync(DateTime date, int page = 1, int amount = 30)
    {
        try
        {
            IEnumerable<Item> items;
            using (var cmd = new HanaCommand(
                "SELECT NAME1 as \"Name\", PRICE as \"Price\", CUTPRICE as \"DiscountPrice\"," +
                " DISCOUNT as \"Discount\", LINK as \"Link\", FETCHDATE as \"FetchDate\", VENDORNAME as \"VendorName\"" +
                $" FROM ITEM_VIEW('PLACEHOLDER' = ('$$IP_date$$', '{date.ToString("yyyy-MM-dd")}'))" +
                $"LIMIT {amount} OFFSET {amount * (page - 1)}",
                _conn))
            {
                items = await ExecuteAndReadQuery(cmd);
            }
            return items;
        }
        catch (HanaException ex)
        {
            var errorMessage = $"couldn't get items from db: {ex.Message}";
            _logger.LogError(errorMessage);
            throw new OperationCanceledException(errorMessage);
        }
    }

    private async Task<IEnumerable<Item>> ExecuteAndReadQuery(HanaCommand cmd)
    {
        var items = new List<Item>();
        using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                items.Add(MapItemFields(reader));
            }
        }
        return items;
    }

    private Item MapItemFields(DbDataReader reader)
    {
        var item = new Item();
        for (var i = 0; i < reader.FieldCount; i++)
        {
            try
            {
                var prop = typeof(Item)?.GetProperty(reader.GetName(i));
                if (prop is null)
                {
                    throw new ArgumentException($"no such field in the model: {reader.GetName(i)}");
                }
                if (reader[i] is HanaDecimal && decimal.TryParse(reader[i].ToString(), out decimal value))
                {
                    prop.SetValue(item, value);
                    continue;
                }
                prop.SetValue(item, reader[i]);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning($"error while mapping item fields: {ex.Message}");
            }
        }
        return item;
    }

    public async Task<IEnumerable<Item>> GetItemsAsync()
    {
        try
        {
            IEnumerable<Item> items;
            using (var cmd = new HanaCommand(
                "SELECT NAME1 as \"Name\", PRICE as \"Price\", CUTPRICE as \"DiscountPrice\"," +
                " DISCOUNT as \"Discount\", LINK as \"Link\", FETCHDATE as \"FetchDate\", VENDORNAME as \"VendorName\"" +
                " FROM ITEM_VIEW",
                _conn))
            {
                items = await ExecuteAndReadQuery(cmd);
            }
            return items;
        }
        catch (HanaException ex)
        {
            var errorMessage = $"couldn't get items from db: {ex.Message}";
            _logger.LogError(errorMessage);
            throw new OperationCanceledException(errorMessage);
        }
    }

    public async Task SetupDbAsync()
    {
        if (_conn is not null)
        {
            await _conn.CloseAsync();
            await _conn.DisposeAsync();
        }
        try
        {
            _conn = new HanaConnection(_dbContext.ConnectionString);
            await _conn.OpenAsync();
        }
        catch (HanaException ex)
        {
            var errorMessage = $"couldn't establish a connection with HANA db: {ex.Message}";
            _logger.LogError(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }
    }

    public string TestMethod()
    {
        return _dbContext.ConnectionString;
    }

    public async Task<int> GetItemsCountAsync()
    {
        try
        {
            using (var cmd = new HanaCommand(
                "SELECT COUNT(*) FROM ITEM_VIEW",
                _conn))
            {
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    await reader.ReadAsync();
                    if (int.TryParse(reader[0].ToString(), out int value))
                    {
                        return value;
                    }
                    return 0;
                }
            }
        }
        catch (HanaException ex)
        {
            var errorMessage = $"couldn't get items from db: {ex.Message}";
            _logger.LogError(errorMessage);
            throw new OperationCanceledException(errorMessage);
        }
    }
}
