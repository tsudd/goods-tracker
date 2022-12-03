namespace GoodsTracker.Platform.Server.Services.Repositories.Abstractions;

public interface IItemRepository
{
    Task<int> GetItemCountAsync();
}