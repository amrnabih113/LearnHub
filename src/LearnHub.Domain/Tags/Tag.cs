using LearnHub.Domain.Common;

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

    public static Tag Create(string name, string slug)
    {
        // Add validation logic for name and slug if needed
        return new Tag(name, slug);
    }

    public void Update(string name, string slug)
    {
        // Add validation logic for name and slug if needed
        Name = name;
        Slug = slug;
    }
}