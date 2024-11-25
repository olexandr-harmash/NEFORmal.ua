namespace NEFORmal.ua.Dating.ApplicationCore.Exceptions;

public class SafeFileException : Exception
{
    // Constructor that accepts only a message
    public SafeFileException(string message)
        : base(message)
    {
    }

    // Constructor that accepts both a message and an inner exception
    public SafeFileException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    // Optionally, you can add custom properties if needed.
    public IEnumerable<string> FileNames { get; set; }
}
