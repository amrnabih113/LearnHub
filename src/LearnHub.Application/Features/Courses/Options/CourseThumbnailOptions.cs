namespace LearnHub.Application.Features.Courses.Options;

public sealed class CourseThumbnailOptions
{
    public const string SectionName = "FileStorage";

    public long MaxImageSizeInBytes { get; set; } = 5 * 1024 * 1024;

    public string[] AllowedImageTypes { get; set; } =
    [
        "image/jpeg",
        "image/png",
        "image/webp"
    ];
}