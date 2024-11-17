namespace NEFORmal.ua.Identity.Api.Exceptions;

public class UserRefreshTokenNotEquivalentException : Exception
{
    public UserRefreshTokenNotEquivalentException() : base("User refresh token are not equivalent.") {}
}