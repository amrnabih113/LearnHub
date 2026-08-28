using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Common.Results;
using Microsoft.Extensions.Options;

using Error = LearnHub.Domain.Common.Results.Error;


namespace LearnHub.Infrastructure.Storage;


public sealed class FileStorageService(
    Cloudinary cloudinary,
    IOptions<FileStorageOptions> options)
        : IFileStorageService
{
    private readonly Cloudinary _cloudinary = cloudinary;
    private readonly FileStorageOptions _options = options.Value;

    public async Task<Result<string>> UploadImageAsync(
        IFileData file,
        string folder,
        CancellationToken cancellationToken = default)
    {

        var validation = ValidateImage(file);


        if (validation.IsError)
        {
            return validation.Errors;
        }



        await using var stream =
            file.OpenReadStream();



        var uploadParams =
            new ImageUploadParams
            {
                File = new FileDescription(
                    file.FileName,
                    stream),

                Folder = folder,

                UseFilename = true,

                UniqueFilename = true,

                Overwrite = false
            };



        var result =
            await _cloudinary.UploadAsync(
                uploadParams,
                cancellationToken);



        if (result.Error is not null)
        {
            return Error.Failure(
                "File.UploadFailed",
                result.Error.Message);
        }



        return result.SecureUrl.ToString();
    }

    public async Task<Result<string>> UploadRawFileAsync(
        byte[] content,
        string fileName,
        string folder,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        if (content is null || content.Length == 0)
        {
            return StorageErrors.EmptyFile;
        }

        using var stream = new MemoryStream(content);
        var uploadParams = new RawUploadParams
        {
            File = new FileDescription(fileName, stream),
            Folder = folder,
            UseFilename = true,
            UniqueFilename = true,
            Overwrite = false
        };

        var result = await _cloudinary.UploadAsync(uploadParams, "raw");
        if (result.Error is not null)
        {
            // Local file storage fallback if Cloudinary fails/unconfigured
            var localDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", folder);
            Directory.CreateDirectory(localDir);
            var localPath = Path.Combine(localDir, fileName);
            await File.WriteAllBytesAsync(localPath, content, cancellationToken);
            return $"/uploads/{folder}/{fileName}";
        }

        return result.SecureUrl.ToString();
    }





    public async Task<Result<Updated>> DeleteImageAsync(
        string imageUrl,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            return Result.Updated;
        }


        var uri = new Uri(imageUrl);


        var segments =
            uri.AbsolutePath.Split('/');


        var uploadIndex =
            Array.IndexOf(
                segments,
                "upload");


        if (uploadIndex == -1)
        {
            return Result.Updated;
        }



        var publicId =
            string.Join(
                '/',
                segments.Skip(uploadIndex + 2));


        publicId =
            Path.ChangeExtension(
                publicId,
                null);



        var deletion =
            await _cloudinary.DestroyAsync(
                new DeletionParams(publicId));



        if (deletion.Error is not null)
        {
            return Error.Failure(
                "File.DeleteFailed",
                deletion.Error.Message);
        }


        return Result.Updated;
    }





    private Result<Updated> ValidateImage(
        IFileData file)
    {
        if (file.Length <= 0)
        {
            return StorageErrors.EmptyFile;
        }



        if (file.Length >
            _options.MaxImageSizeInBytes)
        {
            return StorageErrors.FileTooLarge;
        }



        if (!_options.AllowedImageTypes
            .Contains(
                file.ContentType,
                StringComparer.OrdinalIgnoreCase))
        {
            return StorageErrors.InvalidFileType;
        }



        return Result.Updated;
    }
}