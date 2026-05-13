using Blazored.LocalStorage;
using MAUIERP.ApplicationLayer;
using MAUIERP.ApplicationLayer.Common.Interfaces;
using MAUIERP.BlazorUI;
using MAUIERP.BlazorUI.Services;
using MAUIERP.Infrastructure;
using MAUIERP.Infrastructure.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;
using System.Net;
using System.Reflection;

namespace MAUIERP;

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

        // Load configuration
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream("MAUIERP.BlazorUI.appsettings.json");

        if (stream == null)
            throw new FileNotFoundException("appsettings.json not found");

        var configuration = new ConfigurationBuilder()
            .AddJsonStream(stream)
            .Build();

        builder.Configuration.AddConfiguration(configuration);

        // Fix Android SSL once and for all
        if (DeviceInfo.Current.Platform == DevicePlatform.Android)
        {
            // This bypasses all SSL certificate validation
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) => true;

            // Force older TLS
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        // Register database
        var connectionString = DeviceInfo.Current.Platform == DevicePlatform.Android
            ? builder.Configuration.GetConnectionString("DefaultConnection_Android")
            : builder.Configuration.GetConnectionString("DefaultConnection_Windows");

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null);
            }));

        builder.Services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        // Rest of your services
        builder.Services.AddMauiBlazorWebView();
        builder.Services.AddApplication();
        builder.Services.AddInfrastructureServices(builder.Configuration);
        builder.Services.AddAuthorizationCore();
        builder.Services.AddScoped<CustomAuthenticationStateProvider>();
        builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
            sp.GetRequiredService<CustomAuthenticationStateProvider>());
        builder.Services.AddBlazoredLocalStorage();
        builder.Services.AddMudServices();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}