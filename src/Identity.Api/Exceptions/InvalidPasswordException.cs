namespace NEFORmal.ua.Identity.Api.Exceptions;

public class InvalidPasswordException : Exception
{
    public InvalidPasswordException() : base("Invalid password.") { }
}
