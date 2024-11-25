using NEFORmal.ua.Dating.ApplicationCore.Dtos;
using NEFORmal.ua.Dating.ApplicationCore.Models;

namespace NEFORmal.ua.Dating.ApplicationCore.Interfaces;

public interface IProfileService
{
    Task<Profile?> CreateProfileAsync(CreateProfileDto profile, List<string> fileNames, CancellationToken cancellationToken);
    Task DeleteProfile(string sid, CancellationToken cancellationToken);
    Task<Profile?> UpdateProfile(string sid, UpdateProfileDto profile, List<string> fileNames, CancellationToken cancellationToken);
    Task<ProfileDto> GetProfileById(int profileId, CancellationToken cancellationToken);
    Task<ProfileDto> GetProfileBySid(string sid, CancellationToken cancellationToken);
    Task<IEnumerable<ProfileDto>> GetProfileByFilter(ProfileFilterDto filter, CancellationToken cancellationToken);
}
