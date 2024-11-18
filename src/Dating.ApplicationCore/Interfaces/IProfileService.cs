using NEFORmal.ua.Dating.ApplicationCore.Dtos;
using NEFORmal.ua.Dating.ApplicationCore.Models;

namespace NEFORmal.ua.Dating.ApplicationCore.Interfaces;

public interface IProfileService
{
    Task CreateProfile (CreateProfileDto profile);
    Task DeleteProfile (int profileId, CancellationToken cancellationToken);
    Task UpdateProfile (int profileId, UpdateProfileDto profile, CancellationToken cancellationToken);
    Task<ProfileDto> GetProfileById (int profileId, CancellationToken cancellationToken);
    Task<IEnumerable<ProfileDto>> GetProfileByFilter (ProfileFilterDto filter, CancellationToken cancellationToken);
}