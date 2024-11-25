using NEFORmal.ua.Identity.Api.Models;
using NEFORmal.ua.Identity.Api.Interfaces;
using Microsoft.Extensions.Options;
using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;

namespace NEFORmal.ua.Identity.Api.Services;

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtTokenOptions _options;

    public JwtTokenService(IOptions<JwtTokenOptions> options)
    {
        _options = options.Value;
    }

    public string CreateJwtToken(ApplicationUser user)
    {
        var secretKey = Environment.GetEnvironmentVariable("SECRET");

        var key = Encoding.UTF8.GetBytes(secretKey);
        var secret = new SymmetricSecurityKey(key);

        var signingCredentials = new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name,     user.UserName),
            new Claim(ClaimTypes.Sid,      user.Id),
            new Claim(ClaimTypes.Email,    user.Email),
            new Claim(ClaimTypes.UserData, user.ProfilePhoto ?? "")
        };

        var tokenOptions = new JwtSecurityToken
        (
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_options.Expires),
            signingCredentials: signingCredentials
        );

        var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

        return token;
    }

    public string CreateRefreshToken()
    {
        var rng = new RNGCryptoServiceProvider();
        byte[] randomBytes = new byte[32];
        rng.GetBytes(randomBytes);
        string refreshToken = Convert.ToBase64String(randomBytes);
        return refreshToken;
    }
}
