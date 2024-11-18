using NEFORmal.ua.Identity.Api.Apis;
using NEFORmal.ua.Identity.Api.Interfaces;
using NEFORmal.ua.Identity.Api.Services;

public static class IdentityServicesConfig
{
    public static IServiceCollection ConfigureIdentityServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtTokenOptions>(configuration.GetSection("JwtSettings"));
        
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAuthorizationService, AuthorizationService>();
        services.AddScoped<IdentityServices>();

        return services;
    }
}