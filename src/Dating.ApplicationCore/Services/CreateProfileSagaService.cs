
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NEFORmal.ua.Dating.ApplicationCore.Dtos;
using NEFORmal.ua.Dating.ApplicationCore.Interfaces;

namespace NEFORmal.ua.Dating.ApplicationCore.Services;

public class CreateProfileSagaService : ICreateProfileSagaService
{
    private readonly IProfileService _profileService;
    private readonly IFileService _fileService;
    private List<string> _contextFileNames;

    public CreateProfileSagaService(IProfileService profileService, IFileService fileService)
    {
        _profileService = profileService;
        _fileService = fileService;
    }

    public void Compensate(List<string> formFiles)
    {
        try
        {
            // 3. Компенсация: удаление файлов, если профайл не создан
            _fileService.DeleteFiles(_contextFileNames);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during compensation: {ex.Message}");
        }
    }

    public async Task<bool> ProcessProfileAsync(CreateProfileDto profileForCreate, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Сначала сохраняем файлы, получаем их безопасные имена
            _contextFileNames = await _fileService.SafeFilesOrThrowErrorAsync(profileForCreate.ProfilePhotos);

            // 2. Создаем профиль с результатами сохранения файлов
            var profile = await _profileService.CreateProfileAsync(profileForCreate, _contextFileNames, cancellationToken);
            if (profile == null)
                throw new Exception("Failed to create profile");

            // Если все прошло успешно, возвращаем true
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");

            // Если ошибка произошла, вызываем компенсацию
            Compensate(_contextFileNames);
            return false;
        }
    }
}