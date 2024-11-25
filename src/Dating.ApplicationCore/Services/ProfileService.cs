using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NEFORmal.ua.Dating.ApplicationCore.Dtos;
using NEFORmal.ua.Dating.ApplicationCore.Exceptions;
using NEFORmal.ua.Dating.ApplicationCore.Interfaces;
using NEFORmal.ua.Dating.ApplicationCore.Models;

namespace NEFORmal.ua.Dating.ApplicationCore.Services
{
    /// <summary>
    /// Service responsible for handling operations related to user profiles.
    /// </summary>
    public class ProfileService : IProfileService
    {
        private readonly ILogger<ProfileService> _logger; // Logger for logging messages and errors
        private readonly IProfileRepository _profileRepo; // Repository for interacting with profile data
        private readonly IFileService _fileService; // Service for handling file operations (like profile pictures)

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileService"/> class.
        /// </summary>
        /// <param name="fileService">The file service used for file operations.</param>
        /// <param name="profileRepo">The repository used for profile operations.</param>
        /// <param name="logger">The logger used for logging messages and errors.</param>
        public ProfileService(IFileService fileService, IProfileRepository profileRepo, ILogger<ProfileService> logger)
        {
            _profileRepo = profileRepo;
            _logger = logger;
            _fileService = fileService;
        }

        /// <summary>
        /// Creates a new profile using the provided data.
        /// </summary>
        /// <param name="profileForCreate">The data required to create a profile.</param>
        /// <param name="fileNames">A list of file names for the profile pictures.</param>
        /// <param name="cancellationToken">Token to monitor cancellation of the operation.</param>
        /// <returns>A task representing the result of the asynchronous operation, containing the created profile or null if failed.</returns>
        public async Task<Profile?> CreateProfileAsync(CreateProfileDto profileForCreate, List<string> fileNames, CancellationToken cancellationToken)
        {
            try
            {
                // Create a new profile using the provided data
                var appProfile = new Profile(
                    profileForCreate.Sid,
                    profileForCreate.Name,
                    profileForCreate.Bio,
                    profileForCreate.Age,
                    profileForCreate.Sex,
                    fileNames
                );

                // Save the profile to the repository
                _profileRepo.CreateProfile(appProfile);
                await _profileRepo.SaveChangesAsync(cancellationToken);

                return appProfile; // Return the created profile
            }
            catch (Exception ex)
            {
                // Log the error if any occurs during profile creation
                _logger.LogError($"Error creating profile: {ex.Message}");
                return null; // Return null if profile creation fails
            }
        }

        /// <summary>
        /// Deletes a profile by its unique identifier.
        /// </summary>
        /// <param name="profileId">The unique identifier of the profile to delete.</param>
        /// <param name="cancellationToken">Token to monitor cancellation of the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="Exception">Throws an exception if the profile cannot be found.</exception>
        public async Task DeleteProfile(string sid, CancellationToken cancellationToken)
        {
            var appProfile = await _profileRepo.GetProfileBySid(sid, cancellationToken);

            if (appProfile == null)
            {
                throw new Exception("Profile not found"); // Handle case where profile doesn't exist
            }

            _profileRepo.DeleteProfile(appProfile);
            await _profileRepo.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Retrieves profiles that match the provided filter criteria.
        /// </summary>
        /// <param name="filter">The filter criteria to apply when searching for profiles.</param>
        /// <param name="cancellationToken">Token to monitor cancellation of the operation.</param>
        /// <returns>A task representing the asynchronous operation, containing the list of matching profiles.</returns>
        public async Task<IEnumerable<ProfileDto>> GetProfileByFilter(ProfileFilterDto filter, CancellationToken cancellationToken)
        {
            var profiles = await _profileRepo.GetProfileByFilter(filter, cancellationToken);

            // Map the profile data to a list of ProfileDto
            return profiles.Select(i => new ProfileDto(
                i.Id,
                i.Name,
                i.Bio,
                i.Age,
                i.ProfilePhotos
            ));
        }

        /// <summary>
        /// Retrieves a profile by its unique identifier.
        /// </summary>
        /// <param name="profileId">The unique identifier of the profile to retrieve.</param>
        /// <param name="cancellationToken">Token to monitor cancellation of the operation.</param>
        /// <returns>A task representing the asynchronous operation, containing the profile data.</returns>
        /// <exception cref="Exception">Throws an exception if the profile is not found.</exception>
        public async Task<ProfileDto> GetProfileById(int profileId, CancellationToken cancellationToken)
        {
            var appProfile = await _profileRepo.GetProfileById(profileId, cancellationToken);

            if (appProfile == null)
            {
                throw new Exception("Profile not found"); // Handle case where profile doesn't exist
            }

            return new ProfileDto(
                appProfile.Id,
                appProfile.Name,
                appProfile.Bio,
                appProfile.Age,
                appProfile.ProfilePhotos
            );
        }

        /// <summary>
        /// Retrieves a profile by its session identifier (SID).
        /// </summary>
        /// <param name="sid">The session identifier (SID) used to find the profile.</param>
        /// <param name="cancellationToken">Token to monitor cancellation of the operation.</param>
        /// <returns>A task representing the asynchronous operation, containing the profile data.</returns>
        /// <exception cref="Exception">Throws an exception if the profile is not found.</exception>
        public async Task<ProfileDto> GetProfileBySid(string sid, CancellationToken cancellationToken)
        {
            var appProfile = await _profileRepo.GetProfileBySid(sid, cancellationToken);

            if (appProfile == null)
            {
                throw new Exception("Profile not found"); // Handle case where profile doesn't exist
            }

            return new ProfileDto(
                appProfile.Id,
                appProfile.Name,
                appProfile.Bio,
                appProfile.Age,
                appProfile.ProfilePhotos
            );
        }

        /// <summary>
        /// Updates an existing profile using the provided data.
        /// </summary>
        /// <param name="profileId">The unique identifier of the profile to update.</param>
        /// <param name="profileForUpdate">The data to update the profile with.</param>
        /// <param name="fileNames">A list of file names for updated profile pictures.</param>
        /// <param name="cancellationToken">Token to monitor cancellation of the operation.</param>
        /// <returns>A task representing the asynchronous operation, containing the updated profile or null if update fails.</returns>
        public async Task<Profile?> UpdateProfile(string sid, UpdateProfileDto profileForUpdate, List<string> fileNames, CancellationToken cancellationToken)
        {
            try
            {
                var existingProfile = await _profileRepo.GetProfileBySid(sid, cancellationToken);

                if (existingProfile == null)
                {
                    return null; // Profile not found or SID mismatch
                }

                // Update the profile with new data
                existingProfile.UpdateProfile(
                    profileForUpdate.Name,
                    profileForUpdate.Bio,
                    profileForUpdate.Age,
                    profileForUpdate.Sex,
                    fileNames
                );

                await _profileRepo.SaveChangesAsync(cancellationToken);
                return existingProfile; // Return the updated profile
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating profile: {ex.Message}"); // Log error during update
                return null; // Return null if the update fails
            }
        }
    }
}
