using System.Security.Claims;
using NEFORmal.ua.Identity.Api.Models;

namespace NEFORmal.ua.Identity.Api.Interfaces;

public interface IJwtTokenService
{
    string CreateJwtToken(ApplicationUser user);
    string CreateRefreshToken();
}
