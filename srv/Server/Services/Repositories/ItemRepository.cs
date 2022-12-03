using GoodsTracker.Platform.Server.Services.DbAccess.Abstractions;
using GoodsTracker.Platform.Server.Services.Repositories.Abstractions;

namespace GoodsTracker.Platform.Server.Services.Repositories;

internal sealed class ItemRepository : IItemRepository
{
    private readonly IDbAccess _dbAccess;

    public ItemRepository(IDbAccess dbAccess)
    {
        _dbAccess = dbAccess;
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

    private string GenerateItemCountCommand()
    => $"SELECT COUNT(*) FROM ITEM";
}