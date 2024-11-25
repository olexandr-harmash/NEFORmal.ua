using Microsoft.AspNetCore.Http;

namespace NEFORmal.ua.Dating.ApplicationCore.Dtos;

public record CreateProfileDto(string Sid, string Name, string Sex, string Bio, int Age);