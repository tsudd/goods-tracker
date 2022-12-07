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

    public async Task<BaseInfo> GetItemsInfoAsync()
    {
        try
        {
            var itemsInfo = new BaseInfo();
            using var countReader = await _dbAccess.ExecuteCommandAsync(GenerateItemsCountCommand());
            await countReader.ReadAsync();
            itemsInfo.ItemsCount = countReader.GetInt32(0);
            using var reader = await _dbAccess.ExecuteCommandAsync(GenerateSelectVendorsCommand());
            while (await reader.ReadAsync())
            {
                itemsInfo.ShopsColumns.Add($"{reader.GetInt32(0)},{reader.GetString(1)},{reader.GetString(1)}");
            }
            return itemsInfo;
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
        int vendorFilterId,
        bool discountOnly,
        string? q = null)
    {
        var baseItems = new List<BaseItem>();
        try
        {
            using var reader =
                await _dbAccess.ExecuteCommandAsync(
                    GenerateSelectItemGroupCommand(startIndex, amount, order, vendorFilterId, discountOnly, q));
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

    private string GenerateItemsCountCommand()
    => $"SELECT COUNT(*) FROM ITEM;";
    private string GenerateSelectVendorsCommand()
    => "SELECT ID, NAME1, NAME2 FROM VENDOR;";

    private string GenerateSelectItemGroupCommand(
        int startIndex,
        int amount,
        ItemsOrder order,
        int vendorFilterId,
        bool discountOnly = false,
        string? searchString = null)
    {
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
            + "items.COUNTRY AS \"Country\", "
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
            + (discountOnly ? "WHERE ONDISCOUNT = true" : string.Empty)
            + "   ) AS freshRecords "
            + " WHERE "
            + "   freshRecords.RN = 1 "
            + ") AS records "
            + "LEFT OUTER JOIN ITEM AS items ON records.ITEMID = items.ID "
            + "LEFT OUTER JOIN VENDOR AS vendors ON items.VENDORID = vendors.ID "
            + BuildWhereStatement(searchString, vendorFilterId)
            + BuildOrderByStatement(order)
            + $" LIMIT {amount} OFFSET {startIndex}"
            + ";";
    }

    private static string BuildWhereStatement(string? searchString, int vendorFilterId)
    {
        var whereStatement = new StringBuilder(" WHERE ");
        if (searchString != null && vendorFilterId > 0)
        {
            whereStatement.Append(
                BuildFuzzySearchStatement(searchString)
                + " AND "
                + BuildVendorFilterStatement(vendorFilterId));
        }
        else if (searchString != null)
        {
            whereStatement.Append(BuildFuzzySearchStatement(searchString));
        }
        else if (vendorFilterId > 0)
        {
            whereStatement.Append(BuildVendorFilterStatement(vendorFilterId));
        }
        else
        {
            return string.Empty;
        }
        return whereStatement.ToString();
    }

    private static string BuildFuzzySearchStatement(string searchString)
    => $" CONTAINS(items.NAME1, '{searchString}', FUZZY(0.75,'similarcalculationmode=substringsearch'))";

    private static string BuildVendorFilterStatement(int vendorId)
    => $"VENDORID = {vendorId}";

    private static string BuildOrderByStatement(ItemsOrder order)
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