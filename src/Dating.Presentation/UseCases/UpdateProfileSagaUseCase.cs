
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NEFORmal.ua.Dating.ApplicationCore.Dtos;
using NEFORmal.ua.Dating.ApplicationCore.Interfaces;

namespace NEFORmal.ua.Dating.Presentation.UseCases;

public class UpdateProfileSagaUseCase : IUpdateProfileSagaUseCase
{
    private readonly IProfileService _profileService;
    private readonly IFileService _fileService;
    private readonly ILogger<UpdateProfileSagaUseCase> _logger;
    private List<string> _contextFileNames = new List<string>();
    private IEnumerable<string> _contextLastFileNames = new List<string>();

    public UpdateProfileSagaUseCase(IProfileService profileService, IFileService fileService, ILogger<UpdateProfileSagaUseCase> logger)
    {
        _profileService = profileService;
        _fileService = fileService;
        _logger = logger;
    }

    public void Compensate()
    {
        try
        {
            // 3. Компенсация: удаляем новые файлы, если обновление не удалось
            _fileService.DeleteFiles(_contextFileNames);

            // Восстанавливаем старые файлы, если они были
            if (_contextLastFileNames.Any())
            {
                // Загружаем старые файлы обратно
                //await _fileService.RestoreFilesAsync(currentFileNames);
            }

            _logger.LogError("Files were deleted and old files restored successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error during compensation: {ex.Message}");
        }
    }

    public async Task<bool> UpdateProfileAsync(int profileId, UpdateProfileDto profileForUpdate, List<IFormFile> formFiles, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Сначала сохраняем новые файлы, получаем их безопасные имена
            _contextFileNames = await _fileService.SafeFilesOrThrowErrorAsync(formFiles);

            var profile = await _profileService.UpdateProfile(profileId, profileForUpdate, _contextFileNames, cancellationToken);

            if (profile == null)
                throw new Exception("Failed to update profile");

            Console.WriteLine(profile.LastFiles.First());
            _contextLastFileNames = profile.LastFiles;

            _fileService.DeleteFiles(_contextLastFileNames);

            // Если все прошло успешно, возвращаем true
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error: {ex.Message}");
            // Если ошибка произошла, вызываем компенсацию: удаляем новые файлы и восстанавливаем старые
            Compensate();
            return false;
        }
    }
}