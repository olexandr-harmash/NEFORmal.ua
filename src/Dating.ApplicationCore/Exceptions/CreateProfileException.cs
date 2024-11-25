namespace NEFORmal.ua.Dating.ApplicationCore.Exceptions;

public class CreateProfileException : Exception
{
    // Constructor that accepts only a message
    public CreateProfileException(string message)
        : base(message)
    {
    }

    // Constructor that accepts both a message and an inner exception
    public CreateProfileException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    // Optionally, you can add custom properties if needed.
    public string? ProfileName { get; set; }
    public string? UserId { get; set; }
}
