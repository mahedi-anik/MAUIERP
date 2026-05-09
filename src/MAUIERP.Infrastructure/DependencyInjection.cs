using MAUIERP.ApplicationLayer.Common.Interfaces;
using MAUIERP.Infrastructure.Data;
using MAUIERP.Infrastructure.Options;
using MAUIERP.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace MAUIERP.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // 1. Database (SQLite is recommended for MAUI, but keeping SQL Server if you are connecting to a remote DB)
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

            // 2. JWT Settings (Keep for token generation/parsing logic in IJwtService)
            var jwtSettings = new JwtSettings();
            configuration.GetSection("JwtSettings").Bind(jwtSettings);
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

            // 3. Remove AddAuthentication() and AddJwtBearer() 
            // In MAUI, authentication state is usually managed via a custom AuthenticationStateProvider 
            // in the BlazorUI project.

            // 4. Services
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IDatabaseSeeder, DatabaseSeeder>();

            // 5. Logging (Serilog is compatible with MAUI)
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