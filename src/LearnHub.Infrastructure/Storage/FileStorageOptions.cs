namespace LearnHub.Infrastructure.Storage;

public sealed class FileStorageOptions
{
    public const string SectionName = "FileStorage";
    public long MaxImageSizeInBytes { get; set; } = 5 * 1024 * 1024; // 5 MB

    public string[] AllowedImageTypes { get; set; } =
    [
        "image/jpeg",
        "image/png",
        "image/webp"
    ];
}