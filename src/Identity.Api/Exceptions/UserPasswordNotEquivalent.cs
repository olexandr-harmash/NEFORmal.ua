namespace NEFORmal.ua.Identity.Api.Exceptions;

public class UserPasswordNotEquivalentException : Exception
{
    public UserPasswordNotEquivalentException() : base("User password are not equivalent.") {}
}