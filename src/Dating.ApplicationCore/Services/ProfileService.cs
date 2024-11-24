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

        public async Task CreateProfile(CreateProfileDto profileForCreate)
        {
            var appProfile = new Profile(
                profileForCreate.Sid,
                profileForCreate.Name,
                profileForCreate.Bio,
                profileForCreate.Age,
                profileForCreate.Sex);

            var fileNames = await _safeProfileFilesAsync(profileForCreate.ProfilePhotos);

            try
            {
                appProfile.UpdateProfilePhotos(fileNames);

                _profileRepo.CreateProfile(appProfile);

                await _profileRepo.SaveChangesAsync();
            }
            catch
            {
                _fileService.DeleteFiles(fileNames);
                throw;
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

            await _profileRepo.SaveChangesAsync();
        }

        public async Task<IEnumerable<ProfileDto>> GetProfileByFilter(ProfileFilterDto filter, CancellationToken cancellationToken)
        {
            var profiles = await _profileRepo.GetProfileByFilter(filter, cancellationToken);

            return profiles.Select(i => new ProfileDto(
                i.Id,
                i.Name,
                i.Bio,
                i.Age,
                i.ProfilePhotos));
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
                appProfile.ProfilePhotos);
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
                appProfile.ProfilePhotos);
        }

        public async Task UpdateProfile(int profileId, UpdateProfileDto updateProfileDto, CancellationToken cancellationToken) // TODO: transaction
        {
            var appProfile = await _profileRepo.GetProfileById(profileId, cancellationToken);

            if (appProfile == null)
            {
                throw new Exception();
            }

            var oldFileNames = appProfile.ProfilePhotos;
            
            var fileNames = await _safeProfileFilesAsync(updateProfileDto.ProfilePhotos);

            try
            {
                appProfile.UpdateProfilePhotos(fileNames);

                if (!string.IsNullOrWhiteSpace(updateProfileDto.Bio)) 
                    appProfile.UpdateBio(updateProfileDto.Bio);

                if (!string.IsNullOrWhiteSpace(updateProfileDto.Name)) 
                    appProfile.UpdateName(updateProfileDto.Name);

                if (!string.IsNullOrWhiteSpace(updateProfileDto.Sex)) 
                    appProfile.UpdateSex(updateProfileDto.Sex);

                if (updateProfileDto.Age.HasValue) 
                    appProfile.UpdateAge(updateProfileDto.Age.Value);

                _profileRepo.UpdateProfile(appProfile, cancellationToken);

                await _profileRepo.SaveChangesAsync();

                if (oldFileNames.Any())
                {
                    _fileService.DeleteFiles(oldFileNames);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _fileService.DeleteFiles(fileNames);
                throw;
            }
        }

        private async Task<IEnumerable<string>> _safeProfileFilesAsync(IEnumerable<IFormFile> formFiles)
        {
            // Save the files and get the results
            IEnumerable<FileResult> fileResults = await _fileService.SaveFilesAsync(formFiles);

            // Check if any file had an error
            var errorResult = fileResults.FirstOrDefault(r => r.Error != null);

            if (errorResult != null)
            {
                // Log the error and clean up any files that were already saved
                _fileService.DeleteFiles(fileResults.Select(fr => fr.SafeFilename).ToList());

                // Throw a SafeFileException with the error message and the list of filenames
                throw new SafeFileException(errorResult.Error.Message)
                {
                    FileNames = formFiles.Select(ff => ff.FileName).ToList()
                };
            }

            // If no errors, return the safe filenames of all saved files
            return fileResults.Select(fr => fr.SafeFilename).ToList();
        }
    }
}
