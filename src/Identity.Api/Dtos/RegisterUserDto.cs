namespace NEFORmal.ua.Identity.Api.Dtos;

public record RegisterUserDto
{
    public string Email { get; init; } = string.Empty;
    public string UserName { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
