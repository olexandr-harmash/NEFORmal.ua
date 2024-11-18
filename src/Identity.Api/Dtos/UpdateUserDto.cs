public record UpdateUserDto
{
    public IFormFile? ProfilePhoto { get; init; }
    public string     UserName     { get; init; } = string.Empty;
    public string     Password     { get; init; } = string.Empty;
}