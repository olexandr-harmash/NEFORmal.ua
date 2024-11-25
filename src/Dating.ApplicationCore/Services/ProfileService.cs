using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NEFORmal.ua.Dating.ApplicationCore.Dtos;
using NEFORmal.ua.Dating.ApplicationCore.Exceptions;
using NEFORmal.ua.Dating.ApplicationCore.Interfaces;
using NEFORmal.ua.Dating.ApplicationCore.Models;

namespace NEFORmal.ua.Dating.ApplicationCore.Services
{
    public class ProfileService : IProfileService
    {
        private readonly ILogger<ProfileService> _logger;
        private readonly IProfileRepository _profileRepo;
        private readonly IFileService _fileService;

        public ProfileService(IFileService fileService, IProfileRepository profileRepo, ILogger<ProfileService> logger)
        {
            _profileRepo = profileRepo;
            _logger = logger;
            _fileService = fileService;
        }

        public async Task<Profile?> CreateProfileAsync(CreateProfileDto profileForCreate, List<string> fileNames, CancellationToken cancellationToken)
        {
            try
            {
                var appProfile = new Profile(
                    profileForCreate.Sid,
                    profileForCreate.Name,
                    profileForCreate.Bio,
                    profileForCreate.Age,
                    profileForCreate.Sex,
                    fileNames
                );

                // Сохраняем профиль в репозитории
                _profileRepo.CreateProfile(appProfile);
                await _profileRepo.SaveChangesAsync(cancellationToken);
                //TODO: cancellationToken check
                return appProfile;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task DeleteProfile(int profileId, CancellationToken cancellationToken)
        {
            var appProfile = await _profileRepo.GetProfileById(profileId, cancellationToken);

            if (appProfile == null)
            {
                throw new Exception();
            }

            _profileRepo.DeleteProfile(appProfile);

            await _profileRepo.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<ProfileDto>> GetProfileByFilter(ProfileFilterDto filter, CancellationToken cancellationToken)
        {
            var profiles = await _profileRepo.GetProfileByFilter(filter, cancellationToken);

            return profiles.Select(
                i => new ProfileDto(
                    i.Id,
                    i.Name,
                    i.Bio,
                    i.Age,
                    i.ProfilePhotos
                )
            );
        }

        public async Task<ProfileDto> GetProfileById(int profileId, CancellationToken cancellationToken)
        {
            var appProfile = await _profileRepo.GetProfileById(profileId, cancellationToken);

            if (appProfile == null)
            {
                throw new Exception();
            }

            return new ProfileDto(
                appProfile.Id,
                appProfile.Name,
                appProfile.Bio,
                appProfile.Age,
                appProfile.ProfilePhotos
            );
        }

        public async Task<ProfileDto> GetProfileBySid(string sid, CancellationToken cancellationToken)
        {
            var appProfile = await _profileRepo.GetProfileBySid(sid, cancellationToken);

            if (appProfile == null)
            {
                throw new Exception();
            }

             return new ProfileDto(
                appProfile.Id,
                appProfile.Name,
                appProfile.Bio,
                appProfile.Age,
                appProfile.ProfilePhotos
            );
        }

        public async Task<Profile?> UpdateProfile(int profileId, UpdateProfileDto profileForUpdate, List<string> fileNames, CancellationToken cancellationToken) // TODO: transaction
        {
            try
            {
                var existingProfile = await _profileRepo.GetProfileById(profileId, cancellationToken);
                if (existingProfile == null || existingProfile.Sid != profileForUpdate.sid)
                {
                    return null;
                }

                existingProfile.UpdateProfile(
                    profileForUpdate.Name,
                    profileForUpdate.Bio,
                    profileForUpdate.Age,
                    profileForUpdate.Sex,
                    fileNames
                );

                await _profileRepo.SaveChangesAsync(cancellationToken);
                //TODO: cancellationToken check
                return existingProfile;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating profile: {ex.Message}");
                return null;
            }
        }
    }
}
