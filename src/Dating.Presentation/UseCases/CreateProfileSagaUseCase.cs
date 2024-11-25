using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NEFORmal.ua.Dating.ApplicationCore.Dtos;
using NEFORmal.ua.Dating.ApplicationCore.Interfaces;
using NEFORmal.ua.Dating.Presentation.Requests;

namespace NEFORmal.ua.Dating.Presentation.UseCases
{
    /// <summary>
    /// The use case responsible for handling the profile creation process as part of a saga, ensuring file handling and compensating on failure.
    /// </summary>
    public class CreateProfileSagaUseCase : ICreateProfileSagaUseCase
    {
        private readonly IProfileService _profileService;
        private readonly IFileService _fileService;
        private readonly ILogger<CreateProfileSagaUseCase> _logger;
        private List<string> _contextFileNames = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateProfileSagaUseCase"/> class.
        /// </summary>
        /// <param name="profileService">The service for managing profile creation.</param>
        /// <param name="fileService">The service for handling file operations.</param>
        /// <param name="logger">The logger for logging events and errors.</param>
        public CreateProfileSagaUseCase(IProfileService profileService, IFileService fileService, ILogger<CreateProfileSagaUseCase> logger)
        {
            _profileService = profileService;
            _fileService = fileService;
            _logger = logger;
        }

        /// <summary>
        /// Performs compensation by deleting any files that were created if the profile creation fails.
        /// </summary>
        public void Compensate()
        {
            try
            {
                // 3. Compensation: Delete files if profile creation failed
                _fileService.DeleteFiles(_contextFileNames);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during compensation: {ex.Message}");
            }
        }

        /// <summary>
        /// Processes the creation of a profile, including saving files and creating the profile.
        /// If any error occurs, the compensation logic will be triggered to clean up.
        /// </summary>
        /// <param name="profileForCreate">The profile data to be used for creating the profile.</param>
        /// <param name="formFiles">The files to be associated with the profile.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation if needed.</param>
        /// <returns>A task representing the asynchronous operation. Returns true if profile creation is successful, otherwise false.</returns>
        public async Task<bool> ProcessProfileAsync(CreateProfileDto profileForCreate, List<IFormFile> formFiles, CancellationToken cancellationToken)
        {
            try
            {
                // 1. First, save the files and get their safe names
                _contextFileNames = await _fileService.SafeFilesOrThrowErrorAsync(formFiles);

                // 2. Create the profile with the saved file names
                var profile = await _profileService.CreateProfileAsync(profileForCreate, _contextFileNames, cancellationToken);
                if (profile == null)
                    throw new Exception("Failed to create profile");

                // If everything is successful, return true
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                // If an error occurred, invoke compensation
                Compensate();
                return false;
            }
        }
    }
}
