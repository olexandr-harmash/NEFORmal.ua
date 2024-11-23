using Microsoft.AspNetCore.Http;

namespace NEFORmal.ua.Dating.ApplicationCore.Dtos;

public record CreateProfileDto(string Sid, string Name, string Sex, string Bio, int Age, IEnumerable<IFormFile> ProfilePhotos)
{
    public string Sid { get; init; } = Sid;
    public string Name { get; init; } = Name;
    public string Bio { get; init; } = Bio;
    public int Age { get; init; } = Age;
    public string Sex { get; init; } = Sex;
    public IEnumerable<IFormFile> ProfilePhotos { get; init; } = ProfilePhotos;
}
