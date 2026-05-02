using Blazored.LocalStorage;
using MAUIERP.ApplicationLayer;
using MAUIERP.ApplicationLayer.Common.Interfaces;
using MAUIERP.BlazorUI.Services;
using MAUIERP.Domain.Entities.Auth;
using MAUIERP.Domain.Enums;
using MAUIERP.Infrastructure;
using MAUIERP.Infrastructure.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;
using Serilog;

namespace MAUIERP.BlazorUI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        // Setup logging
        var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var logPath = Path.Combine(currentDirectory, "logs", "app.log");
        var logDirectory = Path.GetDirectoryName(logPath);

        if (!Directory.Exists(logDirectory))
            Directory.CreateDirectory(logDirectory);

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File(logPath, rollingInterval: RollingInterval.Day)
            .WriteTo.Console()
            .CreateLogger();

        Log.Information("=== MAUI ERP Application Starting ===");

        try
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            // Add Serilog
            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog();

            // Configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(currentDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            builder.Services.AddSingleton<IConfiguration>(configuration);

            // Add services
            builder.Services.AddApplication();
            builder.Services.AddInfrastructure(configuration);

            // Add Blazor services
            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddMudServices();

            // Register Authentication services
            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
            builder.Services.AddScoped<CustomAuthenticationStateProvider>();

            Log.Information("All services registered successfully");

            var app = builder.Build();

            // Seed database synchronously (not async)
            try
            {
                using (var scope = app.Services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

                    // Apply migrations
                    context.Database.Migrate();

                    // Seed data synchronously
                    SeedData(context, passwordHasher);
                }
                Log.Information("Database seeded successfully");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error seeding database");
            }

            Log.Information("Application built successfully");

            return app;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application failed to start");
            Log.CloseAndFlush();
            throw;
        }
    }

    private static void SeedData(ApplicationDbContext context, IPasswordHasher passwordHasher)
    {
        // Seed Permissions
        if (!context.Permissions.Any())
        {
            var permissions = new List<Permission>
            {
                new() { Id = Guid.NewGuid(), Code = "COMPANY_VIEW", Name = "View Companies", Module = "Company", Description = "Can view companies", CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
                new() { Id = Guid.NewGuid(), Code = "COMPANY_CREATE", Name = "Create Companies", Module = "Company", Description = "Can create companies", CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
                new() { Id = Guid.NewGuid(), Code = "COMPANY_UPDATE", Name = "Update Companies", Module = "Company", Description = "Can update companies", CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
                new() { Id = Guid.NewGuid(), Code = "COMPANY_DELETE", Name = "Delete Companies", Module = "Company", Description = "Can delete companies", CreatedAt = DateTime.UtcNow, CreatedBy = "System" }
            };
            context.Permissions.AddRange(permissions);
            context.SaveChanges();
        }

        // Seed Role
        if (!context.Roles.Any())
        {
            var adminRole = new Role
            {
                Id = Guid.NewGuid(),
                Name = "Administrator",
                Description = "Full system access",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            };
            context.Roles.Add(adminRole);
            context.SaveChanges();
        }

        // Seed Admin User
        if (!context.Users.Any())
        {
            var adminRole = context.Roles.FirstOrDefault(r => r.Name == "Administrator");
            var adminUser = new User
            {
                Id = Guid.NewGuid(),
                Email = "admin@mauierp.com",
                Username = "admin",
                PasswordHash = passwordHasher.HashPassword("Admin@123"),
                FirstName = "System",
                LastName = "Administrator",
                Status = UserStatus.Active,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            };
            context.Users.Add(adminUser);
            context.SaveChanges();

            if (adminRole != null)
            {
                var userRole = new UserRole
                {
                    UserId = adminUser.Id,
                    RoleId = adminRole.Id,
                    AssignedAt = DateTime.UtcNow,
                    AssignedBy = "System"
                };
                context.Set<UserRole>().Add(userRole);
                context.SaveChanges();
            }
        }
    }
}