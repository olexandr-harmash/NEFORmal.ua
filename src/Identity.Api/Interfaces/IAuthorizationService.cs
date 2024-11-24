using Microsoft.AspNetCore.Identity;
using NEFORmal.ua.Identity.Api.Dtos;

namespace NEFORmal.ua.Identity.Api.Interfaces;

public interface IAuthorizationService
{
    Task DeleteUserAsync(string userid);
    Task<(string, string)> LoginUserAsync(LoginUserDto user);
    Task<(string, string)> RefreshUserAsync(string userid, string refreshToken);
    Task<IdentityResult> RegisterUserAsync(RegisterUserDto user);
    Task<IdentityResult> UpdateUserAsync(string userid, UpdateUserDto user);
}
