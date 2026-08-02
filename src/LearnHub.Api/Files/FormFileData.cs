namespace LearnHub.Api.Files;

using LearnHub.Application.common.Interfaces;
public sealed class FormFileData(IFormFile file)
        : IFileData
{
    private readonly IFormFile _file = file;

    public Stream OpenReadStream()
        => _file.OpenReadStream();

    public string FileName
        => _file.FileName;

    public string ContentType
        => _file.ContentType;

    public long Length
        => _file.Length;
}