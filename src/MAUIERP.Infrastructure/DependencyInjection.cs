using MAUIERP.ApplicationLayer.Common.Interfaces;
using MAUIERP.Infrastructure.Data;
using MAUIERP.Infrastructure.Options;
using MAUIERP.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace MAUIERP.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // 1. Database - REMOVE DbContext registration from here
            // DbContext should be registered in the startup project (MAUIERP.BlazorUI)
            // because it needs platform-specific connection string logic
            // 
            // REMOVE this block:
            // services.AddDbContext<ApplicationDbContext>(options =>
            //     options.UseSqlServer(
            //         configuration.GetConnectionString("DefaultConnection"),
            //         b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
            //
            // services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

            // 2. JWT Settings
            var jwtSettings = new JwtSettings();
            configuration.GetSection("JwtSettings").Bind(jwtSettings);
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

            // 3. Services
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IDatabaseSeeder, DatabaseSeeder>();

            // 4. Logging (Serilog is compatible with MAUI)
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                // Note: File paths on Android/iOS require specific path handling 
                // (e.g., Path.Combine(FileSystem.AppDataDirectory, "log.txt"))
                .CreateLogger();

            services.AddLogging(builder =>
            {
                builder.AddSerilog(dispose: true);
            });

            return services;
        }
    }
}