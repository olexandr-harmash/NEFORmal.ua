namespace NEFORmal.ua.Dating.Presentation.Requests;

public record UpdateProfileRequest(string? Name, string? Sex, string? Bio, int? Age, IEnumerable<IFormFile> ProfilePhotos);