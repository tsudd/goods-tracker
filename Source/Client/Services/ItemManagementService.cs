namespace GoodsTracker.Platform.Client.Services;

using System.Net.Http.Json;
using System.Text;

using FluentResults;

using GoodsTracker.Platform.Client.Constants;
using GoodsTracker.Platform.Client.Constants.Enumerators;
using GoodsTracker.Platform.Client.Models;
using GoodsTracker.Platform.Shared.Models;

public sealed class ItemManagementService
{
    private readonly HttpClient httpClient;

    public ItemManagementService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    internal async Task<Result<InfoModel>> GetInfoAsync()
    {
        try
        {
            return await this.httpClient.GetFromJsonAsync<InfoModel>(
                                 new Uri(GoodsTrackerRoutes.ItemModuleRoute + "/info", UriKind.Relative))
                             .ConfigureAwait(false) ?? Result.Fail<InfoModel>("No info received");
        }
        catch (InvalidOperationException ex)
        {
            return Result.Fail<InfoModel>(ex.Message);
        }
    }

    internal Task<Result<IEnumerable<BaseItemModel>>> GetItemsAsync(int page, string itemsOrder, int shopFilter, bool onlyDiscount)
    {
        var url = new Uri(
            GoodsTrackerRoutes.ItemModuleRoute +
            BuildGetItemsRequestQuery(page, itemsOrder, shopFilter, onlyDiscount), UriKind.Relative);

        return this.GetItemsAsync(url);
    }

    internal Task<Result<IEnumerable<BaseItemModel>>> SearchItemsAsync(
        string q, int page, string itemsOrder, int shopFilter,
        bool onlyDiscount)
    {
        var url = new Uri(
            GoodsTrackerRoutes.ItemModuleRoute +
            "/search" +
            BuildGetItemsRequestQuery(page, itemsOrder, shopFilter, onlyDiscount) +
            $"&q={q}", UriKind.Relative);

        return this.GetItemsAsync(url);
    }

    private async Task<Result<IEnumerable<BaseItemModel>>> GetItemsAsync(Uri url)
    {
        try
        {
            IEnumerable<BaseItemModel>? result = await this.httpClient.GetFromJsonAsync<IEnumerable<BaseItemModel>>(url)
                                                           .ConfigureAwait(false);

            return Result.Ok(result ?? Enumerable.Empty<BaseItemModel>());
        }
        catch (InvalidOperationException ex)
        {
            return Result.Fail("Items request failed." + ex.Message);
        }
    }

    private static string BuildGetItemsRequestQuery(int page, string itemsOrder, int shopFilter, bool onlyDiscount)
    {
        var query = new StringBuilder(
            $"?index={page}&orderBy={itemsOrder}");

        if (shopFilter > 0)
        {
            query.Append($"&shop={shopFilter}");
        }

        if (onlyDiscount)
        {
            query.Append("&onlyDiscount=true");
        }

        return query.ToString();
    }

    internal static string GetShopBadgeStyle(int shopId)
    {
        return shopId % 2 == 1 ? "bg-primary" : "bg-warning";
    }

    internal async Task<Result<ActionResults>> LikeItemAsync(int itemId)
    {
        if (!CurrentUser.IsAuthenticated)
        {
            return Result.Ok(ActionResults.NotAuthenticated);
        }

        HttpResponseMessage result = await this.httpClient.PostAsJsonAsync(
            GoodsTrackerRoutes.ItemModuleRoute + "/like", new ItemLikeModel
            {
                ItemId = itemId,
            }).ConfigureAwait(false);

        return result.IsSuccessStatusCode ? Result.Ok(ActionResults.Success) : Result.Fail("Like failed.");
    }

    internal async Task<Result<ActionResults>> UnLikeItemAsync(int itemId)
    {
        if (!CurrentUser.IsAuthenticated)
        {
            return Result.Ok(ActionResults.NotAuthenticated);
        }

        HttpResponseMessage result = await this.httpClient.DeleteAsync(
            new Uri(GoodsTrackerRoutes.ItemModuleRoute + $"/like/{itemId}")).ConfigureAwait(false);

        return result.IsSuccessStatusCode ? Result.Ok(ActionResults.Success) : Result.Fail("Delete like failed.");
    }
}
