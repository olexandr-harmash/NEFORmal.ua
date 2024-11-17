using NEFORmal.ua.Identity.Api.Dtos;

namespace NEFORmal.ua.Identity.Api.Interfaces;

public interface IAuthorizationService
{
    Task                   DeleteUserAsync   (string userid);
    Task<(string, string)> LoginUserAsync    (LoginUserDto user);
    Task<(string, string)> RefreshUserAsync  (string userid, string refreshToken);
    Task                   RegisterUserAsync (RegisterUserDto user);
}