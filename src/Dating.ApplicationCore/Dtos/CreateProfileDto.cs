using Microsoft.AspNetCore.Http;

namespace NEFORmal.ua.Dating.ApplicationCore.Dtos;

public record CreateProfileDto(string Sid, string Name, string Sex, string Description, int Age, IEnumerable<IFormFile> ProfilePhotos)
{
    public string                 Sid           { get; init; } = Sid;
    public string                 Name          { get; init; } = Name;
    public string                 Description   { get; init; } = Description;
    public int                    Age           { get; init; } = Age;
    public string                 Sex           { get; init; } = Sex;
    public IEnumerable<IFormFile> ProfilePhotos { get; init; } = ProfilePhotos;
}