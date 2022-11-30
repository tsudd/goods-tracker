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
}