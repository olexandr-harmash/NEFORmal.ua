using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

public static class AuthorizationConfiguration
{
    public static IServiceCollection ConfigureAuthorization(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");

        var secretKey = Environment.GetEnvironmentVariable("SECRET");

        ArgumentNullException.ThrowIfNullOrEmpty(secretKey);
        ArgumentNullException.ThrowIfNull(jwtSettings);

        services.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
            };
            options.Events = new JwtBearerEvents
            {
                // Событие при неудачной аутентификации
                OnAuthenticationFailed = context =>
                {
                    Console.WriteLine(context.Exception.Message);
                    return Task.CompletedTask;
                },
                
                // Событие при успешной валидации токена
                OnTokenValidated = context =>
                {
                   Console.WriteLine(context.Principal.Identity.Name);
          
                    return Task.CompletedTask;
                },

                // Событие при отсутствии авторизации (например, токен не передан)
                OnChallenge = context =>
                {
                    Console.WriteLine(context.ErrorDescription);
              
                    return Task.CompletedTask;
                }
            };
        });

        return services;
    }
}
