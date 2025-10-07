using Microsoft.AspNetCore.Components.Authorization;
﻿using Microsoft.Extensions.Logging;
using MetalFlow.Client.Services;
using MudBlazor.Services;

namespace MetalFlow.Client
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

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif
            builder.Services.AddMudServices();

            builder.Services.AddAuthorizationCore();
            builder.Services.AddHttpClient("ServerApi", client =>
            {
                client.BaseAddress = new Uri("https://localhost:7250");
            });

            builder.Services.AddScoped<AuthenticationStateProvider, MauiAuthenticationStateProvider>();
            builder.Services.AddScoped<MauiAuthenticationStateProvider>();

            return builder.Build();
        }
    }
}
