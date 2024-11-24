namespace NEFORmal.ua.Dating.ApplicationCore.Services;

public class FileServiceOptions
{
    public static readonly string OptionString = "FileServiceOptions";

    public static readonly Dictionary<string, List<byte[]>> FileSignature = new Dictionary<string, List<byte[]>>
    {
        { ".jpeg", new List<byte[]>
            {
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE2 },
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE3 },
            }
        },

        { ".webp", new List<byte[]>
            {
                new byte[] { 0x52, 0x49, 0x46, 0x46 }, // RIFF
                new byte[] { 0x57, 0x45, 0x42, 0x50 }, // WEBP
            }
        }
    };

    public string StoredFilesPath { get; init; } = string.Empty;
    public long FileSizeLimit { get; init; }
    public string[] PermittedExtensions { get; init; } = [".webp", ".jpeg"];
}
