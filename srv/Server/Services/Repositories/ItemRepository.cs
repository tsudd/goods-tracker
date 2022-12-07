using System.Data.Common;
using System.Text;
using GoodsTracker.Platform.Server.Entities;
using GoodsTracker.Platform.Server.Services.DbAccess.Abstractions;
using GoodsTracker.Platform.Server.Services.Repositories.Abstractions;
using GoodsTracker.Platform.Server.Services.Repositories.Enumerators;

namespace GoodsTracker.Platform.Server.Services.Repositories;

internal sealed class ItemRepository : IItemRepository
{
    private const string intTypeName = "integer";
    private const string decimalTypeName = "decimal";
    private readonly IDbAccess _dbAccess;
    private readonly ILogger _logger;

    public ItemRepository(IDbAccess dbAccess, ILogger<ItemRepository> logger)
    {
        _dbAccess = dbAccess;
        _logger = logger;
    }

    public async Task<int> GetItemCountAsync()
    {
        try
        {
            using var reader = await _dbAccess.ExecuteCommandAsync(GenerateItemCountCommand());
            if (reader.HasRows)
            {
                await reader.ReadAsync();
                return reader.GetInt32(0);
            }
            throw new InvalidOperationException("couldn't read items count");
        }
        catch (InvalidOperationException ex)
        {
            throw ex;
        }

    }

    public async Task<IEnumerable<BaseItem>> GetItemsByGroupsAsync(
        int startIndex,
        int amount,
        ItemsOrder order,
        string? q = null)
    {
        var baseItems = new List<BaseItem>();
        try
        {
            using var reader =
                await _dbAccess.ExecuteCommandAsync(GenerateSelectItemGroupCommand(startIndex, amount, order, q));
            if (reader.HasRows)
            {
                while (await reader.ReadAsync())
                {
                    baseItems.Add(MapBaseItemFields(reader));
                }
            }
        }
        catch (InvalidOperationException ex)
        {
            throw ex;
        }
        catch (Exception ex)
        {
            _logger.LogCritical($"read error: {ex.Message}");
        }
        return baseItems;
    }

    private BaseItem MapBaseItemFields(DbDataReader reader)
    {
        var baseItemType = typeof(BaseItem);
        var baseItem = new BaseItem();
        for (var i = 0; i < reader.FieldCount; i++)
        {
            try
            {
                var prop = baseItemType.GetProperty(reader.GetName(i));
                if (prop is null)
                {
                    throw new ArgumentException($"no such field in the model: {reader.GetName(i)}");
                }
                if (reader.IsDBNull(i))
                {
                    prop.SetValue(baseItem, null);
                    continue;
                }
                if (reader.GetDataTypeName(i) == decimalTypeName)
                {
                    prop.SetValue(baseItem, reader.GetDecimal(i));
                    continue;
                }
                if (reader.GetDataTypeName(i) == intTypeName)
                {
                    prop.SetValue(baseItem, reader.GetInt32(i));
                    continue;
                }
                prop.SetValue(baseItem, reader[i]);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning($"error while mapping item fields: {ex.Message}");
            }
        }
        return baseItem;
    }

    private string GenerateItemCountCommand()
    => $"SELECT COUNT(*) FROM ITEM";

    private string GenerateSelectItemGroupCommand(int startIndex, int amount, ItemsOrder order, string? searchString = null)
    {
        // TODO: adjust column name COUNTY
        return "SELECT "
            + "records.ITEMID AS \"Id\", "
            + "records.PRICE AS \"Price\", "
            + "records.CUTPRICE AS \"DiscountPrice\", "
            + "records.ONDISCOUNT AS \"OnDiscount\", "
            + "100 - TO_INTEGER("
            + "ROUND("
            + "(records.CUTPRICE / records.PRICE) * 100, 0)) AS \"Discount\", "
            + "items.NAME1 AS \"Name\", "
            + "items.LINK AS \"ImgLink\", "
            + "items.Weight AS \"Weight\", "
            + "items.WEIGHTUNIT AS \"WeightUnit\", "
            + "items.COUNTY AS \"Country\", "
            + "vendors.LAND AS \"Currensy\", "
            + "vendors.NAME1 AS \"VendorName\", "
            + "records.FETCHDATE as \"FetchDate\""
            + "FROM "
            + "("
            + " SELECT "
            + "   freshRecords.ITEMID, "
            + "   freshRecords.FETCHDATE, "
            + "   freshRecords.PRICE, "
            + "   freshRecords.CUTPRICE, "
            + "   freshRecords.ONDISCOUNT "
            + " FROM "
            + "   ( "
            + "     SELECT "
            + "       records.ITEMID, "
            + "       row_number() over("
            + "         partition by records.ITEMID "
            + "         order by "
            + "           streams.FETCHDATE desc "
            + "       ) as rn, "
            + "       streams.FETCHDATE, "
            + "       records.PRICE, "
            + "       records.CUTPRICE, "
            + "       records.ONDISCOUNT "
            + "     FROM "
            + "       ITEMRECORD AS records "
            + "       LEFT OUTER JOIN STREAM AS streams ON streams.ID = records.STREAMID "
            + "   ) AS freshRecords "
            + " WHERE "
            + "   freshRecords.RN = 1 "
            + ") AS records "
            + "LEFT OUTER JOIN ITEM AS items ON records.ITEMID = items.ID "
            + "LEFT OUTER JOIN VENDOR AS vendors ON items.VENDORID = vendors.ID "
            + (searchString != null
            ? $" WHERE CONTAINS(items.NAME1, '{searchString}', FUZZY(0.75,'similarcalculationmode=substringsearch'))"
            : string.Empty)
            + BuildOrderByStatement(order)
            + $" LIMIT {amount} OFFSET {startIndex}"
            + ";";
    }

    private string BuildOrderByStatement(ItemsOrder order)
    {
        if (order == ItemsOrder.None) return string.Empty;

        var orderByStatement = new StringBuilder(" ORDER BY ");
        if (order == ItemsOrder.ByLastUpdateDate)
            orderByStatement.Append("FetchDate DESC");
        else if (order == ItemsOrder.CheapFirst)
            orderByStatement.Append("Price ASC");
        else if (order == ItemsOrder.ExpensiveFirst)
            orderByStatement.Append("Price DESC");
        else throw new InvalidOperationException("couldn't define order of items");
        return orderByStatement.ToString();
    }
}