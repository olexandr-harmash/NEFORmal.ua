namespace NEFORmal.ua.Identity.Api.Exceptions;

public class UserAlreadyExistsException : Exception
{
    public UserAlreadyExistsException() : base("User already exists.") {}
}