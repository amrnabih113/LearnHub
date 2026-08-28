using LearnHub.Domain.Common;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Courses.Enums;
using LearnHub.Domain.Identity;
using LearnHub.Domain.LearningPaths.Enums;

namespace LearnHub.Domain.LearningPaths;

public sealed class LearningPath : AuditableEntity
{
    public string Title { get; private set; } = default!;
    public string Slug { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public string ShortDescription { get; private set; } = default!;
    public string? ThumbnailUrl { get; private set; }
    public CourseLevel Level { get; private set; }
    public LearningPathStatus Status { get; private set; }
    public Guid? OwnerId { get; private set; }
    public User? Owner { get; private set; }
    public DateTimeOffset? PublishedAtUtc { get; private set; }

    private readonly List<LearningPathCourse> _courses = [];
    public IReadOnlyCollection<LearningPathCourse> Courses => _courses.AsReadOnly();

    private LearningPath() { }

    private LearningPath(
        Guid id,
        string title,
        string slug,
        string description,
        string shortDescription,
        string? thumbnailUrl,
        CourseLevel level,
        Guid? ownerId) : base(id)
    {
        Title = title;
        Slug = slug;
        Description = description;
        ShortDescription = shortDescription;
        ThumbnailUrl = thumbnailUrl;
        Level = level;
        Status = LearningPathStatus.Draft;
        OwnerId = ownerId;
    }

    public static Result<LearningPath> Create(
        Guid id,
        string title,
        string? slug,
        string description,
        string shortDescription,
        string? thumbnailUrl,
        CourseLevel level,
        Guid? ownerId)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return LearningPathErrors.TitleRequired;
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            return LearningPathErrors.DescriptionRequired;
        }

        var normalizedTitle = title.Trim();
        var generatedSlug = string.IsNullOrWhiteSpace(slug)
            ? GenerateSlug(normalizedTitle)
            : slug.Trim().ToLowerInvariant();

        return new LearningPath(
            id,
            normalizedTitle,
            generatedSlug,
            description.Trim(),
            shortDescription?.Trim() ?? string.Empty,
            thumbnailUrl?.Trim(),
            level,
            ownerId);
    }

    public Result<Updated> Update(
        string title,
        string? slug,
        string description,
        string shortDescription,
        string? thumbnailUrl,
        CourseLevel level)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return LearningPathErrors.TitleRequired;
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            return LearningPathErrors.DescriptionRequired;
        }

        Title = title.Trim();
        if (!string.IsNullOrWhiteSpace(slug))
        {
            Slug = slug.Trim().ToLowerInvariant();
        }

        Description = description.Trim();
        ShortDescription = shortDescription?.Trim() ?? string.Empty;
        ThumbnailUrl = thumbnailUrl?.Trim();
        Level = level;
        UpdatedAtUtc = DateTimeOffset.UtcNow;

        return Result.Updated;
    }

    public Result<Updated> Publish()
    {
        if (Status == LearningPathStatus.Published)
        {
            return Result.Updated;
        }

        if (_courses.Count == 0)
        {
            return LearningPathErrors.CourseRequired;
        }

        Status = LearningPathStatus.Published;
        PublishedAtUtc = DateTimeOffset.UtcNow;
        UpdatedAtUtc = DateTimeOffset.UtcNow;

        return Result.Updated;
    }

    public Result<Updated> Unpublish()
    {
        if (Status == LearningPathStatus.Draft)
        {
            return Result.Updated;
        }

        Status = LearningPathStatus.Draft;
        UpdatedAtUtc = DateTimeOffset.UtcNow;

        return Result.Updated;
    }

    public Result<Updated> Archive()
    {
        if (Status == LearningPathStatus.Archived)
        {
            return Result.Updated;
        }

        Status = LearningPathStatus.Archived;
        UpdatedAtUtc = DateTimeOffset.UtcNow;

        return Result.Updated;
    }

    public Result<Updated> AddCourse(Guid courseId, int? targetOrder = null, bool isRequired = true)
    {
        if (courseId == Guid.Empty)
        {
            return Error.Validation("LearningPath.CourseIdRequired", "Course ID is required.");
        }

        if (_courses.Any(c => c.CourseId == courseId))
        {
            return LearningPathErrors.CourseAlreadyInPath;
        }

        var nextOrder = targetOrder ?? (_courses.Count + 1);

        var pathCourseResult = LearningPathCourse.Create(Id, courseId, nextOrder, isRequired);
        if (pathCourseResult.IsError)
        {
            return pathCourseResult.Errors;
        }

        _courses.Add(pathCourseResult.Value);
        ReindexCourseOrders();
        UpdatedAtUtc = DateTimeOffset.UtcNow;

        return Result.Updated;
    }

    public Result<Updated> RemoveCourse(Guid courseId)
    {
        var existing = _courses.FirstOrDefault(c => c.CourseId == courseId);
        if (existing is null)
        {
            return LearningPathErrors.CourseNotInPath;
        }

        _courses.Remove(existing);
        ReindexCourseOrders();
        UpdatedAtUtc = DateTimeOffset.UtcNow;

        return Result.Updated;
    }

    public Result<Updated> ReorderCourses(List<Guid> orderedCourseIds)
    {
        if (orderedCourseIds is null || orderedCourseIds.Count != _courses.Count)
        {
            return LearningPathErrors.InvalidOrder;
        }

        if (orderedCourseIds.Distinct().Count() != _courses.Count)
        {
            return LearningPathErrors.InvalidOrder;
        }

        for (int i = 0; i < orderedCourseIds.Count; i++)
        {
            var courseId = orderedCourseIds[i];
            var item = _courses.FirstOrDefault(c => c.CourseId == courseId);
            if (item is null)
            {
                return LearningPathErrors.InvalidOrder;
            }

            item.SetOrder(i + 1);
        }

        _courses.Sort((a, b) => a.Order.CompareTo(b.Order));
        UpdatedAtUtc = DateTimeOffset.UtcNow;

        return Result.Updated;
    }

    private void ReindexCourseOrders()
    {
        _courses.Sort((a, b) => a.Order.CompareTo(b.Order));
        for (int i = 0; i < _courses.Count; i++)
        {
            _courses[i].SetOrder(i + 1);
        }
    }

    private static string GenerateSlug(string text)
    {
        var slug = text.ToLowerInvariant().Trim();
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"\s+", "-").Trim('-');
        return string.IsNullOrEmpty(slug) ? "learning-path" : slug;
    }
}
