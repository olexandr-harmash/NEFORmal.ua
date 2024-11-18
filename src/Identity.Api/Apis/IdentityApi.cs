using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using NEFORmal.ua.Identity.Api.Dtos;
using NEFORmal.ua.Identity.Api.Exceptions;

namespace NEFORmal.ua.Identity.Api.Apis;

public static class IdentityApi
{
    public static WebApplication MapRoutes(WebApplication application)
    {
        var identityGroup = application.MapGroup("authorization");

        identityGroup.MapPut    ("/",        UpdateUserAsync).RequireAuthorization().DisableAntiforgery();
        identityGroup.MapPost   ("/",        RegisterUserAsync);
        identityGroup.MapPost   ("/login",   LoginUserAsync); 
        identityGroup.MapPost   ("/refresh", RefreshUserAsync).RequireAuthorization(); 
        identityGroup.MapDelete ("/",        DeleteUserAsync); 

        return application;
    }

    public static async Task<IResult> UpdateUserAsync(HttpContext httpContext, [FromForm] UpdateUserDto user, IdentityServices services)
    {
        var userIdClaim = httpContext.User.FindFirst(ClaimTypes.Sid);

        if (userIdClaim == null)
        {
            return TypedResults.Unauthorized();
        }

        try
        {   
            await services.AuthorizationService.UpdateUserAsync(userIdClaim.Value, user);

            return TypedResults.Ok();
        }
        catch(Exception e) when (e is InvalidPasswordException || e is UserNotFoundException)
        {
            return TypedResults.BadRequest();
        }
        catch
        {
            return TypedResults.StatusCode(500);
        }
    }

    public static async Task<IResult> RegisterUserAsync(RegisterUserDto user, IdentityServices services)
    {
        try
        {
            var result = await services.AuthorizationService.RegisterUserAsync(user);

            if (!result.Succeeded)
            {
                var errors = result.Errors
                    .Select(error => new { Code = error.Code, Description = error.Description })
                    .ToList();

                return Results.BadRequest(new { Errors = errors });
            }

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

            return TypedResults.Ok(new { JwtToken = jwttoken, RefreshToken = refresh });
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
        var userIdClaim = httpContext.User.FindFirst(ClaimTypes.Sid);

        if (userIdClaim == null)
        {
            return TypedResults.Unauthorized();
        }

        try
        {
            var (jwttoken, refresh) = await services.AuthorizationService.RefreshUserAsync(userIdClaim.Value, refreshToken);

            return TypedResults.Ok(new { JwtToken = jwttoken, RefreshToken = refresh });
        }
        catch (Exception e) when (e is UserRefreshTokenNotEquivalentException || e is UserNotFoundException)
        {
            return TypedResults.BadRequest();
        }
        catch(Exception e)
        {
            return TypedResults.StatusCode(500);
        }
    }
}