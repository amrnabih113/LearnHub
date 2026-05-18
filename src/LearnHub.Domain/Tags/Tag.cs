using LearnHub.Domain.Common;
using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.Courses.Tags;


public sealed class Tag : AuditableEntity
{
    public string Name { get; private set; }
    public string Slug { get; private set; }

    private Tag(string name, string slug)
    {
        Name = name;
        Slug = slug;
    }

    public static Result<Tag> Create(string name, string slug)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return TagErrors.NameRequired;
        }
        if (string.IsNullOrWhiteSpace(slug))
        {
            return TagErrors.SlugRequired;
        }
        return new Tag(name, slug);
    }

    public Result<Updated> Update(string name, string slug)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return TagErrors.NameRequired;
        }
        if (string.IsNullOrWhiteSpace(slug))
        {
            return TagErrors.SlugRequired;
        }
        Name = name;
        Slug = slug;
        return Result.Updated;
    }
}