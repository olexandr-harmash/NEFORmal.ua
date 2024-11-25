using Microsoft.EntityFrameworkCore;
using NEFORmal.ua.Dating.ApplicationCore.Interfaces;
using NEFORmal.ua.Dating.Infrastructure;
using NEFORmal.ua.Dating.Infrastructure.Repository;

namespace NEFORmal.ua.Dating.Presentation.Configuration;

public static class DatabaseConfig
{
    public static async Task<WebApplicationBuilder> ConfigureDatabaseAsync(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("Database");

        ArgumentNullException.ThrowIfNullOrEmpty(connectionString);

        builder.Services.AddDbContext<DatingDbContext>(opt =>
            opt.UseNpgsql(connectionString, opt =>
                opt.MigrationsAssembly("Dating.Infrastructure")
            )
        );

        builder.Services.AddScoped<DatingDbSeed>();
        builder.Services.AddScoped<IProfileRepository, ProfileRepository>();

        if (builder.Environment.IsDevelopment())
        {
            using (var scope = builder.Services.BuildServiceProvider().CreateScope())
            {
                var seeder = scope.ServiceProvider.GetService<DatingDbSeed>();
                await seeder.SeedAsync();
            }
        }

        return builder;
    }
}
