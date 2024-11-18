using Microsoft.Extensions.Logging;

using NEFORmal.ua.Dating.ApplicationCore.Dtos;
using NEFORmal.ua.Dating.ApplicationCore.Models;
using NEFORmal.ua.Dating.ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using NEFORmal.ua.Dating.ApplicationCore.Exceptions;

namespace NEFORmal.ua.Dating.ApplicationCore.Services;

public class ProfileService : IProfileService
{
    private readonly ILogger<ProfileService> _logger;
    private readonly IProfileRepository      _profileRepo;
    private readonly IFileService            _fileService;
    
    public ProfileService(IFileService fileService, IProfileRepository profileRepo, ILogger<ProfileService> logger)
    {
        _profileRepo = profileRepo;
        _logger      = logger;
        _fileService = fileService;
    }

    public async Task CreateProfile(CreateProfileDto profileForCreate)
    {
        var appProfile = new Profile(
            profileForCreate.Sid,
            profileForCreate.Name,
            profileForCreate.Description,
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
        var appprofile = await _profileRepo.GetProfileById(profileId, cancellationToken);

        if (appprofile == null)
        {
            throw new Exception();
        }

        _profileRepo.DeleteProfile(appprofile);

        await _profileRepo.SaveChangesAsync();
    }

    public async Task<IEnumerable<ProfileDto>> GetProfileByFilter(ProfileFilterDto filter, CancellationToken cancellationToken)
    {
        var profiles = await _profileRepo.GetProfileByFilter(filter, cancellationToken);
    
        return profiles.Select(i => new ProfileDto(i.Id, i.Name, i.Bio, i.Age, i.ProfilePhotos));
    }

    public async Task<ProfileDto> GetProfileById(int profileId, CancellationToken cancellationToken)
    {
        var appprofile = await _profileRepo.GetProfileById(profileId, cancellationToken);
        
        if (appprofile == null)
        {
            throw new Exception();
        }

        return new ProfileDto(appprofile.Id, appprofile.Name, appprofile.Bio, appprofile.Age, appprofile.ProfilePhotos);
    }

    public async Task UpdateProfile(int profileId, UpdateProfileDto profile, CancellationToken cancellationToken) //TODO: transaction
    {
        var appProfile = await _profileRepo.GetProfileById(profileId, cancellationToken);
        
        if (appProfile == null)
        {
            throw new Exception();
        }

        var oldFilePaths = appProfile.ProfilePhotos;

        var filePaths = await _safeProfileFilesAsync(profile.ProfilePhotos);

        try
        {
            appProfile.UpdateProfilePhotos(filePaths);

            _profileRepo.UpdateProfile(appProfile, cancellationToken);

            await _profileRepo.SaveChangesAsync();

            if (oldFilePaths.Any())
            {
                _fileService.DeleteFiles(appProfile.ProfilePhotos);
            }
        }
        catch
        {
            _fileService.DeleteFiles(filePaths);
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
            _fileService.DeleteFiles(fileResults.Select(fr => fr.SafeFilename));

            // Throw a SafeFileException with the error message and the list of filenames
            throw new SafeFileException(errorResult.Error.Message)
            {
                FileNames = formFiles.Select(ff => ff.FileName).ToList()
            };
        }

        // If no errors, return the safe filenames of all saved files
        return fileResults.Select(fr => fr.SafeFilename);
    }
}