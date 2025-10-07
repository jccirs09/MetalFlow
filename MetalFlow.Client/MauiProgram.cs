using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Services;
using MetalFlow.Services.Auth;

namespace MetalFlow
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddMudServices();

#if DEBUG
    		builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            builder.Services.AddHttpClient("ServerApi", client =>
            {
                client.BaseAddress = new Uri("https://localhost:7250");
            });

            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped<AuthenticationStateProvider, MauiAuthenticationStateProvider>();
            builder.Services.AddScoped<IAuthService, AuthService>();

            return builder.Build();
        }
    }
}