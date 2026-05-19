using LearnHub.Domain.Common;
using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.Classification.Tags;

public sealed class Tag : AuditableEntity
{
    public string Name { get; private set; } = default!;
    public string Slug { get; private set; } = default!;
    public string? Description { get; private set; }

    private Tag() { }

    private Tag(Guid id, string name, string slug, string? description) : base(id)
    {
        Name = name;
        Slug = slug;
        Description = description;
    }

    public static Result<Tag> Create(Guid id, string name, string slug, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return TagErrors.NameRequired;
        }

        if (string.IsNullOrWhiteSpace(slug))
        {
            return TagErrors.SlugRequired;
        }

        return new Tag(id, name.Trim(), NormalizeSlug(slug), description?.Trim());
    }

    public Result<Updated> Rename(string name, string slug, string? description = null)
    {
        

        if (string.IsNullOrWhiteSpace(name))
        {
            return TagErrors.NameRequired;
        }

        if (string.IsNullOrWhiteSpace(slug))
        {
            return TagErrors.SlugRequired;
        }

        Name = name.Trim();
        Slug = NormalizeSlug(slug);
        Description = description?.Trim();
        UpdatedAtUtc = DateTimeOffset.UtcNow;
        return Result.Updated;
    }



    private static string NormalizeSlug(string value)
    {
        return value.Trim().ToLowerInvariant().Replace(' ', '-');
    }
}
