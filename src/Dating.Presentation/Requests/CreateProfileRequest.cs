namespace NEFORmal.ua.Dating.Presentation.Requests;

public record CreateProfileRequest(string Name, string Sex, string Bio, int Age, IEnumerable<IFormFile> ProfilePhotos);