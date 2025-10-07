using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Maui.Storage;

namespace MetalFlow.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly NavigationManager _navigationManager;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public AuthService(IHttpClientFactory httpClientFactory, NavigationManager navigationManager, AuthenticationStateProvider authenticationStateProvider)
        {
            _httpClient = httpClientFactory.CreateClient("ServerApi");
            _navigationManager = navigationManager;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            var loginRequest = new { email, password };
            var response = await _httpClient.PostAsJsonAsync("login", loginRequest);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResult>();
                if (result?.AccessToken != null)
                {
                    await SecureStorage.SetAsync("authToken", result.AccessToken);
                    ((MauiAuthenticationStateProvider)_authenticationStateProvider).NotifyUserAuthentication(result.AccessToken);
                    _navigationManager.NavigateTo("/");
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> RegisterAsync(string email, string password)
        {
            var registerRequest = new { email, password };
            var response = await _httpClient.PostAsJsonAsync("register", registerRequest);

            return response.IsSuccessStatusCode;
        }

        public async Task LogoutAsync()
        {
            SecureStorage.Remove("authToken");
            ((MauiAuthenticationStateProvider)_authenticationStateProvider).NotifyUserLogout();
            _navigationManager.NavigateTo("/login");
            await Task.CompletedTask;
        }

        private class LoginResult
        {
            public string? AccessToken { get; set; }
        }
    }
}