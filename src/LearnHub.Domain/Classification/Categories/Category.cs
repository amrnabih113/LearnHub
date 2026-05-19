using LearnHub.Domain.Common;
using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.Classification.Categories;

public sealed class Category : AuditableEntity
{
    public string Name { get; private set; } = default!;
    public string Slug { get; private set; } = default!;
    public string? Description { get; private set; }
    public Guid? ParentCategoryId { get; private set; }

    private Category() { }

    private Category(Guid id, string name, string slug, string? description, Guid? parentCategoryId) : base(id)
    {
        Name = name;
        Slug = slug;
        Description = description;
        ParentCategoryId = parentCategoryId;
    }

    public static Result<Category> Create(Guid id, string name, string slug, string? description = null, Guid? parentCategoryId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return CategoryErrors.NameRequired;
        }

        if (string.IsNullOrWhiteSpace(slug))
        {
            return CategoryErrors.SlugRequired;
        }

        if (parentCategoryId.HasValue && parentCategoryId.Value == Guid.Empty)
        {
            return CategoryErrors.ParentCategoryRequired;
        }

        return new Category(id, name.Trim(), NormalizeSlug(slug), description?.Trim(), parentCategoryId);
    }

    public Result<Updated> Rename(string name, string slug, string? description = null)
    {

        if (string.IsNullOrWhiteSpace(name))
        {
            return CategoryErrors.NameRequired;
        }

        if (string.IsNullOrWhiteSpace(slug))
        {
            return CategoryErrors.SlugRequired;
        }

        Name = name.Trim();
        Slug = NormalizeSlug(slug);
        Description = description?.Trim();
        UpdatedAtUtc = DateTimeOffset.UtcNow;
        return Result.Updated;
    }

    public Result<Updated> ChangeParent(Guid? parentCategoryId, IReadOnlyCollection<Guid>? ancestorCategoryIds = null)
    {

        if (parentCategoryId.HasValue && parentCategoryId.Value == Id)
        {
            return CategoryErrors.HierarchyInvalid;
        }

        if (ancestorCategoryIds is not null && ancestorCategoryIds.Contains(Id))
        {
            return CategoryErrors.HierarchyInvalid;
        }

        ParentCategoryId = parentCategoryId;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
        return Result.Updated;
    }


    private static string NormalizeSlug(string value)
    {
        return value.Trim().ToLowerInvariant().Replace(' ', '-');
    }
}
