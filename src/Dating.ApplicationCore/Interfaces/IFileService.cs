using Microsoft.AspNetCore.Http;
using NEFORmal.ua.Dating.ApplicationCore.Services;

namespace NEFORmal.ua.Dating.ApplicationCore.Interfaces;

public interface IFileService
{
    Task<FileResult> SaveFileAsync(IFormFile file);
    void DeleteFile(string filePath);
    Task<IEnumerable<FileResult>> SaveFilesAsync(IEnumerable<IFormFile> file);
    void DeleteFiles(IEnumerable<string> filePaths);
    Task<List<string>> SafeFilesOrThrowErrorAsync(IEnumerable<IFormFile> formFiles);
}
