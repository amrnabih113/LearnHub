namespace LearnHub.Infrastructure.Storage;

using LearnHub.Domain.Common.Results;

public static class StorageErrors
{
    public static readonly Error FileRequired =
        Error.Validation("File.Required", "File is required.");

    public static readonly Error EmptyFile =
        Error.Validation("File.Empty", "File is empty.");

    public static readonly Error FileTooLarge =
        Error.Validation("File.TooLarge", "Maximum image size is 5 MB.");

    public static readonly Error InvalidFileType =
        Error.Validation("File.InvalidType", "Only JPG, JPEG, PNG and WEBP images are allowed.");

    public static readonly Error InvalidFileExtension =
        Error.Validation("File.InvalidExtension", "Unsupported image extension.");

    public static readonly Error UploadFailed =
        Error.Failure("File.UploadFailed", "Failed to upload image.");

    public static readonly Error DeleteFailed =
        Error.Failure("File.DeleteFailed", "Failed to delete image.");
}