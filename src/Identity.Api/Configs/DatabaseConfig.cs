using Microsoft.EntityFrameworkCore;

namespace NEFORmal.ua.Identity.Api.Configs;

public static class DatabaseConfig
{
    public static IServiceCollection ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");

        ArgumentNullException.ThrowIfNullOrEmpty(connectionString);

        services.AddDbContext<ApplicationDbContext>(opt =>
            opt.UseNpgsql(connectionString, opt =>
                opt.MigrationsAssembly("Identity.Api")
            )
        );

        return services;
    }
}
