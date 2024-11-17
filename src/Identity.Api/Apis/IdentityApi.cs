using System.Security.Claims;
using NEFORmal.ua.Identity.Api.Dtos;
using NEFORmal.ua.Identity.Api.Exceptions;

namespace NEFORmal.ua.Identity.Api.Apis;

public static class IdentityApi
{
    public static WebApplication MapRoutes(WebApplication application)
    {
        var identityGroup = application.MapGroup("authorization");

        identityGroup.MapPost   ("/",        RegisterUserAsync);
        identityGroup.MapPost   ("/login",   LoginUserAsync); 
        identityGroup.MapPost   ("/refresh", RefreshUserAsync).RequireAuthorization(); 
        identityGroup.MapDelete ("/",        DeleteUserAsync); 

        return application;
    }

    public static async Task<IResult> RegisterUserAsync(RegisterUserDto user, IdentityServices services)
    {
        try
        {
            await services.AuthorizationService.RegisterUserAsync(user);

            return TypedResults.Ok();
        }
        catch
        {
            return TypedResults.StatusCode(500);
        }
    }

    public static async Task<IResult> LoginUserAsync(LoginUserDto user, IdentityServices services)
    {
        try
        {
            var (jwttoken, refresh) = await services.AuthorizationService.LoginUserAsync(user);

            return TypedResults.Ok((jwttoken, refresh));
        }
        catch(Exception e) when (e is UserPasswordNotEquivalentException || e is ArgumentNullException || e is UserNotFoundException)
        {
            return TypedResults.BadRequest();
        }
        catch
        {
            return TypedResults.StatusCode(500);
        }
    }

    public static async Task<IResult> DeleteUserAsync(string userid, IdentityServices services)
    {
        try
        {
            await services.AuthorizationService.DeleteUserAsync(userid);

            return TypedResults.NoContent();
        }
        catch(UserNotFoundException)
        {
            return TypedResults.BadRequest();
        }
        catch
        {
            return TypedResults.StatusCode(500);
        }
    }

    public static async Task<IResult> RefreshUserAsync(HttpContext httpContext, string refreshToken, IdentityServices services)
    {
        try
        {
            var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return TypedResults.Unauthorized();
            }

            var (jwttoken, refresh) = await services.AuthorizationService.RefreshUserAsync(userIdClaim.Value, refreshToken);

            return TypedResults.Ok((jwttoken, refresh));
        }
        catch (Exception e) when (e is UserRefreshTokenNotEquivalentException || e is UserNotFoundException)
        {
            return TypedResults.BadRequest();
        }
        catch
        {
            return TypedResults.StatusCode(500);
        }
    }
}