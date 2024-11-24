using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NEFORmal.ua.Dating.ApplicationCore.Exceptions;
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
            UnsafeFilename = Path.GetFileNameWithoutExtension(formFile.FileName),
            FileSize = formFile.Length,
            MimeType = Path.GetExtension(formFile.FileName),
            SafeFilename = Path.GetRandomFileName(),
            FullFilename = formFile.FileName
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

        var ext = fileResult.MimeType.TrimStart('.');

        if (string.IsNullOrEmpty(ext) || !_options.PermittedExtensions.Contains(ext))
        {
            fileResult.Error = new NotSupportedException($"File extension '{ext}' is not allowed.");
            return fileResult;
        }

        var filePath = Path.Combine(_options.StoredFilesPath, $"{fileResult.SafeFilename}{fileResult.MimeType}");

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

    public async Task<List<string>> SafeFilesOrThrowErrorAsync(IEnumerable<IFormFile> formFiles)
    {
        var getNames = (IEnumerable<IFormFile> formFiles) => formFiles.Select(f => f.FileName).ToList();
        
        // Save the files and get the results
        IEnumerable<FileResult> fileResults = await SaveFilesAsync(formFiles);

        // Check if any file had an error
        var errorResult = fileResults.FirstOrDefault(r => r.Error != null);

        if (errorResult != null)
        {
            // Throw a SafeFileException with the error message and the list of filenames
            throw new SafeFileException(errorResult.Error.Message)
            {
                FileNames = getNames(formFiles)
            };
        }

        // If no errors, return the safe filenames of all saved files
        return getNames(formFiles);
    }
}

public record FileResult
{
    public string UnsafeFilename { get; set; }
    public string SafeFilename { get; set; }
    public string MimeType { get; set; }
    public string FullFilename { get; set; }
    public long FileSize { get; set; }
    public Exception? Error { get; set; } = default;
}
