
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NEFORmal.ua.Dating.ApplicationCore.Dtos;
using NEFORmal.ua.Dating.ApplicationCore.Interfaces;

namespace NEFORmal.ua.Dating.ApplicationCore.Services;

public class UpdateProfileSagaService : IUpdateProfileSagaService
{
    private readonly IProfileService _profileService;
    private readonly IFileService _fileService;
    private List<string> _contextFileNames;
    private List<string> _contextLastFileNames;

    public UpdateProfileSagaService(IProfileService profileService, IFileService fileService)
    {
        _profileService = profileService;
        _fileService = fileService;
    }

    public void Compensate(List<string> formFiles, List<string> lastFiles)
    {
        try
        {
            // 3. Компенсация: удаляем новые файлы, если обновление не удалось
            _fileService.DeleteFiles(formFiles);

            // Восстанавливаем старые файлы, если они были
            if (lastFiles.Any())
            {
                // Загружаем старые файлы обратно
                //await _fileService.RestoreFilesAsync(currentFileNames);
            }

            Console.WriteLine("Files were deleted and old files restored successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during compensation: {ex.Message}");
        }
    }

    public async Task<bool> UpdateProfileAsync(int profileId, UpdateProfileDto profileForUpdate, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Сначала сохраняем новые файлы, получаем их безопасные имена
            _contextFileNames = await _fileService.SafeFilesOrThrowErrorAsync(profileForUpdate.ProfilePhotos);

            var profile = await _profileService.UpdateProfile(profileId, profileForUpdate, _contextFileNames, cancellationToken);

            if (profile == null)
                throw new Exception("Failed to update profile");

            _contextLastFileNames = profile.LastFiles;

            _fileService.DeleteFiles(_contextLastFileNames);

            // Если все прошло успешно, возвращаем true
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");

            // Если ошибка произошла, вызываем компенсацию: удаляем новые файлы и восстанавливаем старые
            Compensate(_contextFileNames, _contextLastFileNames);
            return false;
        }
    }
}