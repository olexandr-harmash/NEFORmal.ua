namespace NEFORmal.ua.Dating.ApplicationCore.Dtos;

public record ProfileDto(int Id, string Name, string Description, int Age, IEnumerable<string> ProfilePhotos);