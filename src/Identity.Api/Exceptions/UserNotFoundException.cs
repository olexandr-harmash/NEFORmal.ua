namespace NEFORmal.ua.Identity.Api.Exceptions;

public class UserNotFoundException : Exception
{
    public UserNotFoundException() : base("User not found.") {}
}