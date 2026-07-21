using LearnHub.Domain.Common.Results;

namespace LearnHub.Application.common.Interfaces;

public interface IFileStorageService
{
    Task<Result<string>> UploadImageAsync(
       IFileData file,
       string folder,
       CancellationToken cancellationToken = default);

    Task<Result<Updated>> DeleteImageAsync(
        string imageUrl,
        CancellationToken cancellationToken = default);
}