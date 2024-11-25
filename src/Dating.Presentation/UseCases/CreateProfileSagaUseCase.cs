
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NEFORmal.ua.Dating.ApplicationCore.Dtos;
using NEFORmal.ua.Dating.ApplicationCore.Interfaces;
using NEFORmal.ua.Dating.Presentation.Requests;

namespace NEFORmal.ua.Dating.Presentation.UseCases;

public class CreateProfileSagaUseCase : ICreateProfileSagaUseCase
{
    private readonly IProfileService _profileService;
    private readonly IFileService _fileService;
    private readonly ILogger<CreateProfileSagaUseCase> _logger;
    private List<string> _contextFileNames = new List<string>();

    public CreateProfileSagaUseCase(IProfileService profileService, IFileService fileService, ILogger<CreateProfileSagaUseCase> logger)
    {
        _profileService = profileService;
        _fileService = fileService;
        _logger = logger;
    }

    public void Compensate()
    {
        try
        {
            // 3. Компенсация: удаление файлов, если профайл не создан
            _fileService.DeleteFiles(_contextFileNames);
        }
        catch (Exception ex)
        {
           _logger.LogError($"Error during compensation: {ex.Message}");
        }
    }

    public async Task<bool> ProcessProfileAsync(CreateProfileDto profileForCreate, List<IFormFile> formFiles, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Сначала сохраняем файлы, получаем их безопасные имена
            _contextFileNames = await _fileService.SafeFilesOrThrowErrorAsync(formFiles);

            // 2. Создаем профиль с результатами сохранения файлов
            var profile = await _profileService.CreateProfileAsync(profileForCreate, _contextFileNames, cancellationToken);
            if (profile == null)
                throw new Exception("Failed to create profile");

            // Если все прошло успешно, возвращаем true
            return true;
        }
        catch (Exception ex)
        {
           _logger.LogError($"Error: {ex.Message}");
            // Если ошибка произошла, вызываем компенсацию
            Compensate();
            return false;
        }
    }
}