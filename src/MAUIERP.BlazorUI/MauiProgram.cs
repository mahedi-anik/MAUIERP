using Blazored.LocalStorage;
using MAUIERP.ApplicationLayer;
using MAUIERP.BlazorUI;
using MAUIERP.BlazorUI.Services;
using MAUIERP.Infrastructure;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;

namespace MAUIERP;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        #region MAUI

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        #endregion

        #region Configuration

        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        builder.Configuration.AddConfiguration(configuration);

        #endregion

        #region Blazor Hybrid

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        #endregion

        #region Application + Infrastructure

        builder.Services.AddApplication();

        builder.Services.AddInfrastructure(configuration);

        #endregion

        #region Authentication

        builder.Services.AddAuthorizationCore();

        // DO NOT use AddCascadingAuthenticationState()

        builder.Services.AddScoped<CustomAuthenticationStateProvider>();

        builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
            sp.GetRequiredService<CustomAuthenticationStateProvider>());

        #endregion

        #region Local Storage

        builder.Services.AddBlazoredLocalStorage();

        #endregion

        #region UI Services

        builder.Services.AddMudServices();

        #endregion

        return builder.Build();
    }
}