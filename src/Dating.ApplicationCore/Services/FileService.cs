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

    // Constructor that injects the logger and options for the file service.
    public FileService(ILogger<FileService> logger, IOptions<FileServiceOptions> options)
    {
        _logger = logger;
        _options = options.Value;
    }

    /// <summary>
    /// Asynchronously saves a single file and returns a result with the file's information.
    /// </summary>
    public async Task<FileResult> SaveFileAsync(IFormFile file)
    {
        ArgumentNullException.ThrowIfNull(file); // Ensure file is not null

        var fileResult = await _saveFileAsync(file); // Save the file asynchronously

        // If an error occurred during the file save, log the error
        if (fileResult.Error != null)
        {
            _logger.LogError($"Error saving file '{file.FileName}': {fileResult.Error.Message}");
        }

        return fileResult; // Return the result, containing either the safe filename or an error
    }

    /// <summary>
    /// Asynchronously saves multiple files and returns a list of results.
    /// </summary>
    public async Task<IEnumerable<FileResult>> SaveFilesAsync(IEnumerable<IFormFile> files)
    {
        var fileResults = new List<FileResult>();

        foreach (var file in files)
        {
            fileResults.Add(await _saveFileAsync(file)); // Save each file and add the result to the list
        }

        return fileResults;
    }

    /// <summary>
    /// Deletes a single file from storage.
    /// </summary>
    public void DeleteFile(string fileName)
    {
        try
        {
            var filePath = Path.Combine(_options.StoredFilesPath, fileName);

            if (File.Exists(filePath)) // If file exists, delete it
            {
                File.Delete(filePath);
            }
        }
        catch (Exception deleteEx)
        {
            // Log any errors encountered during the file deletion
            _logger.LogError($"Error deleting file {fileName}: {deleteEx.Message}");
        }
    }

    /// <summary>
    /// Deletes multiple files from storage.
    /// </summary>
    public void DeleteFiles(IEnumerable<string> fileNames)
    {
        foreach (var fileName in fileNames)
        {
            DeleteFile(fileName); // Call the DeleteFile method for each file
        }
    }

    // Helper method to handle the actual file saving process with validation and file writing.
    private async Task<FileResult> _saveFileAsync(IFormFile formFile)
    {
        var fileResult = new FileResult
        {
            UnsafeFilename = Path.GetFileNameWithoutExtension(formFile.FileName),
            FileSize = formFile.Length,
            MimeType = Path.GetExtension(formFile.FileName),
            SafeFilename = Path.GetRandomFileName(), // Generate a unique safe filename
            FullFilename = formFile.FileName
        };

        // Validate if the file is empty
        if (formFile.Length == 0)
        {
            fileResult.Error = new ArgumentException("File cannot be empty.", nameof(formFile));
            return fileResult;
        }

        // Validate if the file exceeds the size limit
        if (formFile.Length > _options.FileSizeLimit)
        {
            fileResult.Error = new ArgumentOutOfRangeException(nameof(formFile), "File size exceeds the limit.");
            return fileResult;
        }

        // Validate if the file's extension is allowed
        var ext = fileResult.MimeType;

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
                // Validate the file's signature if required
                if (_options.ValidateFileSignature)
                {
                    var result = ValidateSignature(stream, ext);

                    if (!result)
                    {
                        fileResult.Error = new InvalidOperationException("File signature validation failed.");
                        return fileResult;
                    }
                }

                await formFile.CopyToAsync(stream); // Save the file content
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            fileResult.Error = new InvalidOperationException("An error occurred while saving the file.", ex);
        }

        return fileResult;
    }

    /// <summary>
    /// Validates the file's signature based on the provided file stream and extension.
    /// </summary>
    public bool ValidateSignature(Stream stream, string ext)
    {
        using (var reader = new BinaryReader(stream))
        {
            var signatures = FileServiceOptions.FileSignature[ext]; // Get the valid signatures for the file extension
            var headerBytes = reader.ReadBytes(signatures.Max(m => m.Length));

            // Check if any signature matches the start of the file
            return signatures.Any(signature =>
                headerBytes.Take(signature.Length).SequenceEqual(signature));
        }
    }

    /// <summary>
    /// Asynchronously saves files and throws an exception if any file has an error.
    /// </summary>
    public async Task<List<string>> SafeFilesOrThrowErrorAsync(IEnumerable<IFormFile> formFiles)
    {
        var getNames = (IEnumerable<FileResult> fileResults) => fileResults.Select(f => f.SafeFilename + f.MimeType).ToList();
       
        // Save the files and get the results
        IEnumerable<FileResult> fileResults = await SaveFilesAsync(formFiles);

        // Check if any file had an error
        var errorResult = fileResults.FirstOrDefault(r => r.Error != null);

        if (errorResult != null)
        {
            // Throw a SafeFileException with the error message and the list of filenames
            throw new SafeFileException(errorResult.Error.Message)
            {
                FileNames = getNames(fileResults)
            };
        }

        // If no errors, return the safe filenames of all saved files
        return getNames(fileResults);
    }
}

// Represents the result of a file operation, including information like the original filename, safe filename, and any errors encountered.
public record FileResult
{
    public string UnsafeFilename { get; set; } // The original filename without extension
    public string SafeFilename { get; set; }   // The randomized, safe filename
    public string MimeType { get; set; }       // The file's MIME type (extension)
    public string FullFilename { get; set; }   // The full original filename with extension
    public long FileSize { get; set; }         // The size of the file
    public Exception? Error { get; set; } = default; // Any error that occurred during the file operation
}
