using LearnHub.Domain.Common;
using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.Courses.Sections.Lessons.Resources;

public class Resource : AuditableEntity
{
    public string Name { get; private set; } = default!;
    public string Url { get; private set; } = default!;
    public ResourceType Type { get; private set; }
    public long SizeInBytes { get; private set; }

    private Resource() { }

    private Resource(Guid id, string name, string url, ResourceType type, long sizeInBytes) : base(id)
    {
        Name = name;
        Url = url;
        Type = type;
        SizeInBytes = sizeInBytes;
    }
    public static Result<Resource> Create(Guid id, string name, string url, ResourceType type, long sizeInBytes)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return ResourceErrors.NameRequired;
        }
        if (string.IsNullOrWhiteSpace(url))
        {
            return ResourceErrors.UrlRequired;
        }
        if (!Enum.IsDefined(typeof(ResourceType), type))
        {
            return ResourceErrors.InvalidResourceType;
        }
        if (sizeInBytes < 0)
        {
            return ResourceErrors.InvalidSize;
        }

        return new Resource(id, name, url, type, sizeInBytes);
    }
    public Result<Updated> Update(string name, string url, ResourceType type, long sizeInBytes)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return ResourceErrors.NameRequired;
        }
        if (string.IsNullOrWhiteSpace(url))
        {
            return ResourceErrors.UrlRequired;
        }
        if (!Enum.IsDefined(typeof(ResourceType), type))
        {
            return ResourceErrors.InvalidResourceType;
        }
        if (sizeInBytes < 0)
        {
            return ResourceErrors.InvalidSize;
        }

        Name = name;
        Url = url;
        Type = type;
        SizeInBytes = sizeInBytes;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
        return Result.Updated;
    }
}
