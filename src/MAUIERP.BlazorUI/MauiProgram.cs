using Blazored.LocalStorage;
using MAUIERP.ApplicationLayer.Common.Interfaces;
using MAUIERP.BlazorUI;
using MAUIERP.BlazorUI.Services;
using MAUIERP.Infrastructure.Data;
using MAUIERP.Infrastructure.Options;
using MAUIERP.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;
using System.Reflection;

namespace MAUIERP
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

            // Add authentication services
            builder.Services.AddAuthorizationCore();
            builder.Services.AddCascadingAuthenticationState();

            // Register CustomAuthenticationStateProvider
            builder.Services.AddScoped<CustomAuthenticationStateProvider>();
            builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
                sp.GetRequiredService<CustomAuthenticationStateProvider>());

            // Add Blazored LocalStorage
            builder.Services.AddBlazoredLocalStorage();

            // Register MediatR - CRITICAL for Login page
            builder.Services.AddMediatR(cfg => {
                // Register from Application Layer (where LoginCommand is)
                cfg.RegisterServicesFromAssembly(typeof(MAUIERP.ApplicationLayer.Features.Auth.Commands.LoginCommand).Assembly);
            });

            // Register Database Context
            var connectionString = "Server=DESKTOP-NF2UF0M\\SQLEXPRESS01;Database=MAUIERPDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";
            builder.Services.AddDbContext<IApplicationDbContext, ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Register Application Layer Services
            builder.Services.AddScoped<IApplicationDbContext>(sp =>
                sp.GetRequiredService<ApplicationDbContext>());
            builder.Services.AddScoped<IJwtService, JwtService>();
            builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

            // Add configuration
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            builder.Configuration.AddConfiguration(configuration);

            // Configure JWT settings from appsettings.json
            builder.Services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            builder.Services.AddSingleton<JwtSettings>(sp =>
                configuration.GetSection("JwtSettings").Get<JwtSettings>());

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            builder.Services.AddMudServices();

            return builder.Build();
        }
    }
}