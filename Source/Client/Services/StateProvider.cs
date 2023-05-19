using System.Security.Claims;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using GoodsTracker.Platform.Client.Models;

namespace GoodsTracker.Platform.Client.Services;

public sealed class StateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService localStorage;
    private readonly HttpClient httpClient;

    public StateProvider(ILocalStorageService localStorage, HttpClient httpClient)
    {
        this.localStorage = localStorage;
        this.httpClient = httpClient;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var identity = new ClaimsIdentity();
        string? token = await this.localStorage.GetItemAsStringAsync("accessToken").ConfigureAwait(false);
        this.httpClient.DefaultRequestHeaders.Authorization = null;

        try
        {
            if (CurrentUser.IsAuthenticated)
            {
                if (!string.IsNullOrEmpty(token))
                {
                    this.httpClient.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.Replace("\"", ""));
                }

                string? userId = await this.localStorage.GetItemAsync<string>("userId").ConfigureAwait(false);

                if (userId == CurrentUser.Uid)
                {
                    var claims = new List<Claim>
                    {
                        new(ClaimTypes.Name, CurrentUser.UserName ?? throw new ArgumentException()),
                        new(ClaimTypes.Role, CurrentUser.Role)
                    };

                    identity = new ClaimsIdentity(claims, "authentication");

                    return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity)))
                                     .ConfigureAwait(false);
                }
                else
                {
                    var claims = new List<Claim>
                    {
                        new(ClaimTypes.Name, CurrentUser.UserName ?? throw new ArgumentException()),
                        new(ClaimTypes.Role, "Basic User"),
                    };

                    identity = new ClaimsIdentity(claims, "authentication");

                    return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity)))
                                     .ConfigureAwait(false);
                }
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine(@"Request failed:" + ex.Message);
        }

        AuthenticationState authState = new(new ClaimsPrincipal(identity));

        return authState;
    }

    internal void ManageUser()
    {
        this.NotifyAuthenticationStateChanged(this.GetAuthenticationStateAsync());
    }
}
