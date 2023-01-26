using System.Security.Claims;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using GoodsTracker.Platform.Client.Models;

namespace GoodsTracker.Platform.Client.Services;

public class StateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;
    private readonly HttpClient _httpClient;

    public StateProvider(ILocalStorageService localStorage, HttpClient httpClient)
    {
        _localStorage = localStorage;
        _httpClient = httpClient;
    }

    public async override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var identity = new ClaimsIdentity();
        var token = await _localStorage.GetItemAsStringAsync("accessToken");

        _httpClient.DefaultRequestHeaders.Authorization = null;
        try
        {
            if (CurrentUser.IsAuthenticated)
            {
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.Replace("\"", ""));
                }
                var _userId = await _localStorage.GetItemAsync<string>("userId");
                if (_userId == CurrentUser.Uid)
                {
                    var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, CurrentUser.UserName ?? throw new ArgumentException()),
                            new Claim(ClaimTypes.Role, CurrentUser.Role)
                        };
                    identity = new ClaimsIdentity(claims, "authentication");
                    return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity)));
                }
                else
                {
                    var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, CurrentUser.UserName ?? throw new ArgumentException()),
                            new Claim(ClaimTypes.Role, "Basic User")
                        };
                    identity = new ClaimsIdentity(claims, "authentication");
                    return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity)));
                }
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine("Request failed:" + ex.ToString());
        }
        AuthenticationState authState = new AuthenticationState(new ClaimsPrincipal(identity));
        return authState;
    }

    public void ManageUser()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}