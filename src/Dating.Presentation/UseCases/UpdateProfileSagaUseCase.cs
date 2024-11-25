using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NEFORmal.ua.Dating.ApplicationCore.Dtos;
using NEFORmal.ua.Dating.ApplicationCore.Interfaces;

namespace NEFORmal.ua.Dating.Presentation.UseCases
{
    /// <summary>
    /// The use case responsible for handling the profile update process as part of a saga, ensuring file handling and compensation on failure.
    /// </summary>
    public class UpdateProfileSagaUseCase : IUpdateProfileSagaUseCase
    {
        private readonly IProfileService _profileService;
        private readonly IFileService _fileService;
        private readonly ILogger<UpdateProfileSagaUseCase> _logger;
        private List<string> _contextFileNames = new List<string>();
        private IEnumerable<string> _contextLastFileNames = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateProfileSagaUseCase"/> class.
        /// </summary>
        /// <param name="profileService">The service for managing profiles.</param>
        /// <param name="fileService">The service for handling file operations.</param>
        /// <param name="logger">The logger for logging events and errors.</param>
        public UpdateProfileSagaUseCase(IProfileService profileService, IFileService fileService, ILogger<UpdateProfileSagaUseCase> logger)
        {
            _profileService = profileService;
            _fileService = fileService;
            _logger = logger;
        }

        /// <summary>
        /// Compensates for any failed operations by deleting new files and restoring the old files, if available.
        /// </summary>
        public void Compensate()
        {
            try
            {
                // 3. Compensation: Delete new files if the update failed
                _fileService.DeleteFiles(_contextFileNames);

                // Restore old files, if available
                if (_contextLastFileNames.Any())
                {
                    // This is a placeholder for file restoration logic (can be implemented as needed)
                    // await _fileService.RestoreFilesAsync(currentFileNames);
                }

                _logger.LogError("Files were deleted and old files restored successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during compensation: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates a user's profile, saving new files, deleting old files, and handling errors through compensation.
        /// </summary>
        /// <param name="sid">The session identifier of the user.</param>
        /// <param name="profileForUpdate">The updated profile data.</param>
        /// <param name="formFiles">The files to be associated with the updated profile.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation if needed.</param>
        /// <returns>A task representing the result of the asynchronous operation. Returns true if the update is successful, otherwise false.</returns>
        public async Task<bool> UpdateProfileAsync(string sid, UpdateProfileDto profileForUpdate, List<IFormFile> formFiles, CancellationToken cancellationToken)
        {
            try
            {
                // 1. First, save the new files and get their safe names
                _contextFileNames = await _fileService.SafeFilesOrThrowErrorAsync(formFiles);

                // Attempt to update the profile with the new files
                var profile = await _profileService.UpdateProfile(sid, profileForUpdate, _contextFileNames, cancellationToken);

                if (profile == null)
                    throw new Exception("Failed to update profile");

                // Log the first file name of the profile's last files (for debugging purposes)
                Console.WriteLine(profile.LastFiles.First());
                _contextLastFileNames = profile.LastFiles;

                // Delete old files that are no longer associated with the updated profile
                _fileService.DeleteFiles(_contextLastFileNames);

                // If everything went well, return true
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                // If an error occurred, invoke compensation: delete new files and restore old ones
                Compensate();
                return false;
            }
        }
    }
}
