using Microsoft.AspNetCore.Identity;

using NEFORmal.ua.Identity.Api.Dtos;
using NEFORmal.ua.Identity.Api.Models;
using NEFORmal.ua.Identity.Api.Interfaces;
using NEFORmal.ua.Identity.Api.Exceptions;

namespace NEFORmal.ua.Identity.Api.Services;

public class AuthorizationService : IAuthorizationService
{
    private readonly IJwtTokenService             _tokenService;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthorizationService(UserManager<ApplicationUser> userManager, IJwtTokenService tokenService)
    {
        _userManager  = userManager;
        _tokenService = tokenService;
    }

    public async Task DeleteUserAsync(string userid)
    {
        var appuser = await _userManager.FindByIdAsync(userid);

        if (appuser == null)
        {
            throw new UserNotFoundException();
        }

        await _userManager.DeleteAsync(appuser);
    }

    public async Task<(string, string)> LoginUserAsync(LoginUserDto user)
    {
        var email = user.Email;
        var login = user.UserName;

        if (string.IsNullOrEmpty(email) && string.IsNullOrEmpty(login))
        {
            throw new ArgumentNullException();
        }

        ApplicationUser? appuser;

        if (string.IsNullOrEmpty(email))
        {
            appuser = await _userManager.FindByNameAsync(login);
        }
        else
        {
            appuser = await _userManager.FindByEmailAsync(email);
        }

        if (appuser == null)
        {
            throw new UserNotFoundException();
        }

        var passresult = await _userManager.CheckPasswordAsync(appuser, user.Password);

        if (!passresult)
        {
            throw new UserPasswordNotEquivalentException();
        }

        var jwttoken = _tokenService.CreateJwtToken(appuser);
        var refresh  = _tokenService.CreateRefreshToken();
        
        return (jwttoken, refresh);
    }

    public async Task<(string, string)> RefreshUserAsync(string userid, string refreshToken)
    {
        var appuser = await _userManager.FindByIdAsync(userid);

        if (appuser == null)
        {
            throw new UserNotFoundException();
        }

        if (appuser.RefreshToken != refreshToken)
        {
            throw new UserRefreshTokenNotEquivalentException();
        }

        var jwttoken = _tokenService.CreateJwtToken(appuser);
        var refresh  = _tokenService.CreateRefreshToken();

        appuser.RefreshToken = refresh;

        await _userManager.UpdateAsync(appuser);

        return (jwttoken, refresh);
    }

    public async Task RegisterUserAsync(RegisterUserDto user)
    {
        var appuser = await _userManager.FindByNameAsync(user.UserName);

        if (appuser != null)
        {
            throw new UserAlreadyExistsException();
        }

        await _userManager.CreateAsync(new ApplicationUser{ UserName = user.UserName, Email = user.Email });
    }
}