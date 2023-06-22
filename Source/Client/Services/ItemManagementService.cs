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
    private InfoModel? info;

    public ItemManagementService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    internal async Task<Result<InfoModel>> GetInfoAsync()
    {
        // TODO: better cache handling
        try
        {
            if (this.info != null)
            {
                return Result.Ok(this.info);
            }

            Result<InfoModel>? getInfoResult = await this.httpClient.GetFromJsonAsync<InfoModel>(
                                                             new Uri(GoodsTrackerRoutes.ItemModuleRoute + "/info", UriKind.Relative))
                                                         .ConfigureAwait(false) ??
                                               Result.Fail<InfoModel>("No info received");

            if (getInfoResult.IsSuccess)
            {
                this.info = getInfoResult.Value;
            }
            else
            {
                return getInfoResult;
            }

            return Result.Ok(this.info);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Fail<InfoModel>(ex.Message);
        }
    }

    internal async Task<Result<ItemModel>> GetItemDetailsAsync(int id)
    {
        try
        {
            return await this.httpClient.GetFromJsonAsync<ItemModel>(
                                 new Uri(GoodsTrackerRoutes.ItemModuleRoute + $"/{id}", UriKind.Relative))
                             .ConfigureAwait(false) ??
                   Result.Fail<ItemModel>("No item received");
        }
        catch (InvalidOperationException ex)
        {
            return Result.Fail<ItemModel>(ex.Message);
        }
    }

    internal Task<Result<IEnumerable<BaseItemModel>>> GetItemsAsync(
        int page, string itemsOrder, int shopFilter, bool onlyDiscount)
    {
        var url = new Uri(
            GoodsTrackerRoutes.ItemModuleRoute + BuildGetItemsRequestQuery(page, itemsOrder, shopFilter, onlyDiscount),
            UriKind.Relative);

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
        var query = new StringBuilder($"?index={page}&orderBy={itemsOrder}");

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
                                                   GoodsTrackerRoutes.ItemModuleRoute + "/like",
                                                   new ItemLikeModel
                                                   {
                                                       ItemId = itemId,
                                                   })
                                               .ConfigureAwait(false);

        return result.IsSuccessStatusCode ? Result.Ok(ActionResults.Success) : Result.Fail("Like failed.");
    }

    internal async Task<Result<ActionResults>> UnLikeItemAsync(int itemId)
    {
        if (!CurrentUser.IsAuthenticated)
        {
            return Result.Ok(ActionResults.NotAuthenticated);
        }

        HttpResponseMessage result = await this.httpClient.DeleteAsync(
                                                   new Uri(
                                                       GoodsTrackerRoutes.ItemModuleRoute +
                                                       $"/like/{itemId}", UriKind.Relative))
                                               .ConfigureAwait(false);

        return result.IsSuccessStatusCode ? Result.Ok(ActionResults.Success) : Result.Fail("Delete like failed.");
    }

    internal async Task<Result<IEnumerable<BaseItemModel>>> GetFavorites()
    {
        var url = new Uri(GoodsTrackerRoutes.FavoritesModuleRoute, UriKind.Relative);

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
        catch (HttpRequestException ex)
        {
            return Result.Fail("Items request failed." + ex.Message);
        }
    }
}
