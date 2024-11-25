using Microsoft.AspNetCore.Http;

namespace NEFORmal.ua.Dating.ApplicationCore.Dtos;

public record UpdateProfileDto(string sid, string? Name, string? Sex, string? Bio, int? Age);