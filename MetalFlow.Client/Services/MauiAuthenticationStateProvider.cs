using System.Security.Claims;
using System.Text.Json;
using MetalFlow.Client.Data;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Json;

namespace MetalFlow.Client.Services;

public class MauiAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());

    public MauiAuthenticationStateProvider(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var token = await SecureStorage.GetAsync("access_token");

            if (string.IsNullOrEmpty(token))
            {
                return new AuthenticationState(_anonymous);
            }

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var identity = new ClaimsIdentity(jwtToken.Claims, "jwt");
            var user = new ClaimsPrincipal(identity);

            return new AuthenticationState(user);
        }
        catch
        {
            return new AuthenticationState(_anonymous);
        }
    }

    public async Task<bool> LoginAsync(LoginModel model)
    {
        var httpClient = _httpClientFactory.CreateClient("ServerApi");
        var response = await httpClient.PostAsJsonAsync("/login", new { model.Email, model.Password });

        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        var result = await response.Content.ReadFromJsonAsync<JsonElement>();
        var token = result.GetProperty("accessToken").GetString();

        if (string.IsNullOrEmpty(token))
        {
            return false;
        }

        await SecureStorage.SetAsync("access_token", token);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        return true;
    }

    public async Task LogoutAsync()
    {
        SecureStorage.Remove("access_token");
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
    }
}