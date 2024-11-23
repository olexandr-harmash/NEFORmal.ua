using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using NEFORmal.ua.Dating.ApplicationCore.Interfaces;

namespace NEFORmal.ua.Dating.ApplicationCore.Services;

public class FileService : IFileService
{
    private readonly ILogger<FileService> _logger;
    private readonly FileServiceOptions _options;

    public FileService(ILogger<FileService> logger, IOptions<FileServiceOptions> options)
    {
        _logger = logger;
        _options = options.Value;
    }

    public async Task<FileResult> SaveFileAsync(IFormFile file)
    {
        ArgumentNullException.ThrowIfNull(file);

        var fileResult = await _saveFileAsync(file);

        if (fileResult.Error != null)
        {
            _logger.LogError($"Error saving file '{file.FileName}': {fileResult.Error.Message}");
        }

        return fileResult; // Return the safe filename if no error occurred
    }

    public async Task<IEnumerable<FileResult>> SaveFilesAsync(IEnumerable<IFormFile> files)
    {
        var fileResults = new List<FileResult>();

        foreach (var file in files)
        {
            fileResults.Add(await _saveFileAsync(file)); // Add safe filename to the list
        }

        return fileResults;
    }

    public void DeleteFile(string fileName)
    {
        try
        {
            var filePath = Path.Combine(_options.StoredFilesPath, fileName);

            if (!File.Exists(filePath))
            {
                return;
            }

            File.Delete(filePath);
        }
        catch (Exception deleteEx)
        {
            _logger.LogError($"Error deleting file {fileName}: {deleteEx.Message}");
        }
    }

    public void DeleteFiles(IEnumerable<string> fileNames)
    {
        foreach (var fileName in fileNames)
        {
            DeleteFile(fileName);
        }
    }

    private async Task<FileResult> _saveFileAsync(IFormFile formFile)
    {
        var fileResult = new FileResult
        {
            UnsafeFilename = formFile.Name,
            FileSize = formFile.Length,
        };

        if (formFile.Length == 0)
        {
            fileResult.Error = new ArgumentException("File cannot be empty.", nameof(formFile));
            return fileResult;
        }

        if (formFile.Length > _options.FileSizeLimit)
        {
            fileResult.Error = new ArgumentOutOfRangeException(nameof(formFile), "File size exceeds the limit.");
            return fileResult;
        }

        var ext = Path.GetExtension(formFile.FileName).ToLowerInvariant();

        if (string.IsNullOrEmpty(ext) || !_options.PermittedExtensions.Contains(ext))
        {
            fileResult.Error = new NotSupportedException($"File extension '{ext}' is not allowed.");
            return fileResult;
        }

        var filename = Path.GetRandomFileName();
        var filePath = Path.Combine(_options.StoredFilesPath, filename);

        fileResult.SafeFilename = filename;

        try
        {
            using (var stream = File.Create(filePath))
            {
                var result = ValidateSignature(stream, ext);

                if (!result)
                {
                    fileResult.Error = new InvalidOperationException("File signature validation failed.");
                    return fileResult;
                }

                await formFile.CopyToAsync(stream);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            fileResult.Error = new InvalidOperationException("An error occurred while saving the file.", ex);
        }

        return fileResult;
    }

    public bool ValidateSignature(Stream stream, string ext)
    {
        using (var reader = new BinaryReader(stream))
        {
            var signatures = FileServiceOptions.FileSignature[ext];
            var headerBytes = reader.ReadBytes(signatures.Max(m => m.Length));

            return signatures.Any(signature =>
                headerBytes.Take(signature.Length).SequenceEqual(signature));
        }
    }
}

public record FileResult
{
    public string UnsafeFilename { get; set; } = string.Empty;
    public string? SafeFilename { get; set; }
    public long FileSize { get; set; }
    public Exception? Error { get; set; } = default;
}
