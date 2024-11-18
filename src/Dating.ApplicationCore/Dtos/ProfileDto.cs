namespace NEFORmal.ua.Dating.ApplicationCore.Dtos;

public record ProfileDto(int Id, string Name, string Description, int Age, IEnumerable<string> ProfilePhotos)
{
    public int                 Id            { get; init; } = Id;
    public string              Name          { get; init; } = Name;
    public string              Description   { get; init; } = Description;
    public int                 Age           { get; init; } = Age;
    public IEnumerable<string> ProfilePhotos { get; init; } = ProfilePhotos;
}