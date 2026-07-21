namespace LearnHub.Application.common.Interfaces;

public interface IFileData
{
    Stream OpenReadStream();

    string FileName { get; }

    string ContentType { get; }

    long Length { get; }
}