using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Components.Authorization;
using MetalFlow.Client.Models;
using System.Text.Json.Serialization;

namespace MetalFlow.Client.Services;

public class MauiAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly IHttpClientFactory _httpClientFactory;
    private ClaimsPrincipal _anonymous = new(new ClaimsIdentity());

    public MauiAuthenticationStateProvider(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var token = await SecureStorage.GetAsync("authToken");
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

    public async Task<bool> LoginAsync(LoginModel loginModel)
    {
        var httpClient = _httpClientFactory.CreateClient("ServerApi");
        var response = await httpClient.PostAsJsonAsync("login", new { email = loginModel.Email, password = loginModel.Password });

        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
        if (tokenResponse?.AccessToken == null)
        {
            return false;
        }

        await SecureStorage.SetAsync("authToken", tokenResponse.AccessToken);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        return true;
    }

    public async Task LogoutAsync()
    {
        SecureStorage.Remove("authToken");
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
    }

    private class TokenResponse
    {
        [JsonPropertyName("accessToken")]
        public string? AccessToken { get; set; }
    }
}