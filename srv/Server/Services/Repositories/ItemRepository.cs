using System.Data.Common;
using GoodsTracker.Platform.Server.Entities;
using GoodsTracker.Platform.Server.Services.DbAccess.Abstractions;
using GoodsTracker.Platform.Server.Services.Repositories.Abstractions;

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

    public async Task<IEnumerable<BaseItem>> GetItemsByGroupsAsync(int page, int amount)
    {
        var baseItems = new List<BaseItem>();
        try
        {
            using var reader = await _dbAccess.ExecuteCommandAsync(GenerateSelectItemGroupCommand(page, amount));
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

    private string GenerateSelectItemGroupCommand(int page, int amount)
    => "SELECT records.ITEMID AS \"Id\""
    + ",records.PRICE AS \"Price\""
    + ",records.CUTPRICE AS \"DiscountPrice\""
    + ",records.ONDISCOUNT AS \"OnDiscount\""
    + ",100 - TO_INTEGER(ROUND((records.CUTPRICE / records.PRICE) * 100, 0)) AS \"Discount\""
    + ",items.NAME1 AS \"Name\""
    + ",items.LINK as \"ImgLink\""
    + ",items.WEIGHT as \"Weight\""
    + ",items.WEIGHTUNIT as \"WeightUnit\""
    + ",items.COUNTY as \"Country\""
    + ",vendors.LAND as \"Currency\""
    + ",vendors.NAME1 as \"VendorName\""
    + ",MAX(streams.FETCHDATE) AS \"FetchDate\""
    + " FROM ITEMRECORD AS records"
    + " LEFT JOIN ITEM AS items ON records.ITEMID = items.ID"
    + " LEFT JOIN STREAM AS streams ON records.STREAMID = streams.ID"
    + " LEFT JOIN VENDOR AS vendors ON items.VENDORID = vendors.ID"
    + " GROUP BY records.ITEMID"
    + ",items.NAME1"
    + ",records.PRICE"
    + ",records.ONDISCOUNT"
    + ",records.CUTPRICE"
    + ",items.WEIGHT"
    + ",items.WEIGHTUNIT"
    + ",items.COUNTY"
    + ",items.LINK"
    + ",vendors.LAND"
    + $",vendors.NAME1 LIMIT {amount} OFFSET {amount * (page - 1)};";
}