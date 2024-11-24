namespace NEFORmal.ua.Identity.Api.Dtos;

public record LoginUserDto
{
    public string? Email { get; init; } = string.Empty;
    public string? UserName { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
