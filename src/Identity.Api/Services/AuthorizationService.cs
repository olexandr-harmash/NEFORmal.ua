using Microsoft.AspNetCore.Identity;

using NEFORmal.ua.Identity.Api.Dtos;
using NEFORmal.ua.Identity.Api.Models;
using NEFORmal.ua.Identity.Api.Interfaces;
using NEFORmal.ua.Identity.Api.Exceptions;

namespace NEFORmal.ua.Identity.Api.Services;

public class AuthorizationService : IAuthorizationService
{
    private readonly IJwtTokenService _tokenService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<AuthorizationService> _logger;

    public AuthorizationService(UserManager<ApplicationUser> userManager, IJwtTokenService tokenService, ILogger<AuthorizationService> logger)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _logger = logger;
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
        var refresh = _tokenService.CreateRefreshToken();

        appuser.RefreshToken = refresh;

        await _userManager.UpdateAsync(appuser);

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
        var refresh = _tokenService.CreateRefreshToken();

        appuser.RefreshToken = refresh;

        await _userManager.UpdateAsync(appuser);

        return (jwttoken, refresh);
    }

    public async Task<IdentityResult> RegisterUserAsync(RegisterUserDto user)
    {
        var appuser = await _userManager.FindByNameAsync(user.UserName);

        if (appuser != null)
        {
            throw new UserAlreadyExistsException();
        }

        var newuser = new ApplicationUser
        {
            UserName = user.UserName,
            Email = user.Email
        };

        return await _userManager.CreateAsync(newuser, user.Password);
    }

    public async Task<IdentityResult> UpdateUserAsync(string userid, UpdateUserDto user)
    {
        var appuser = await _userManager.FindByIdAsync(userid);

        if (appuser == null)
        {
            throw new UserNotFoundException();
        }

        if (user.Password != null)
        {
            var passwordValid = await _userManager.CheckPasswordAsync(appuser, user.Password);

            if (!passwordValid)
            {
                throw new InvalidPasswordException();
            }

            appuser.PasswordHash = _userManager.PasswordHasher.HashPassword(appuser, user.Password);
        }

        string? filePath = null;

        if (user.ProfilePhoto != null)
        {
            filePath = await SaveFileAsync(user.ProfilePhoto);
            appuser.ProfilePhoto = filePath;
        }

        if (user.UserName != null)
        {
            appuser.UserName = user.UserName;
        }

        var result = await _userManager.UpdateAsync(appuser);

        if (!result.Succeeded)
        {
            if (filePath != null)
            {
                try
                {
                    File.Delete(filePath);
                }
                catch (Exception deleteEx)
                {
                    _logger.LogError($"Error deleting file: {deleteEx.Message}");
                }
            }
        }

        return result;
    }

    private async Task<string> SaveFileAsync(IFormFile file)
    {
        var filename = $"{Guid.NewGuid()}_{file.FileName}";

        try
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", filename);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return filePath;
        }
        catch
        {
            throw new SaveFileException();
        }
    }
}
