using MetalFlow.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;

namespace MetalFlow.Client;

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

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        // Add MudBlazor services
        builder.Services.AddMudServices();

        // Add Authentication services
        builder.Services.AddAuthorizationCore();
        builder.Services.AddScoped<AuthenticationStateProvider, MauiAuthenticationStateProvider>();
        builder.Services.AddScoped<MauiAuthenticationStateProvider>();

        // Register HttpClient
        builder.Services.AddHttpClient("ServerApi", client =>
        {
            client.BaseAddress = new Uri("https://localhost:7157");
        });

        return builder.Build();
    }
}