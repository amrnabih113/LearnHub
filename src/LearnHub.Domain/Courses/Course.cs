using LearnHub.Domain.Common;

using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Identity;
using LearnHub.Domain.Courses.Events;
using LearnHub.Domain.Courses.Enums;
using LearnHub.Domain.Subscriptions;
using LearnHub.Domain.Purchasing.ValueObjects;
using LearnHub.Domain.Common.ValueObjects;
using LearnHub.Domain.Courses.Sections;
using LearnHub.Domain.Classification.Tags;
using LearnHub.Domain.Classification.Categories;

namespace LearnHub.Domain.Courses;

public sealed class Course : AuditableEntity
{
    public string? Title { get; private set; }
    public string? Description { get; private set; }
    public Guid? InstructorId { get; private set; }
    public Guid CategoryId { get; private set; }

    public User? Instructor { get; private set; }
    private readonly List<Guid> _tagIds = [];
    public IReadOnlyCollection<Guid> TagIds => _tagIds.AsReadOnly();
    public ICollection<CourseTag> CourseTags { get; private set; } = []; public Category? Category { get; private set; }
    private readonly List<Section> _sections = [];
    public IEnumerable<Section> Sections => _sections.AsReadOnly();
    public string? ThumbnailUrl { get; private set; }
    public Language Language { get; private set; } = null!;
    public bool IsIncludedInSubscription { get; private set; }
    public SubscriptionTier RequiredSubscriptionTier { get; private set; }

    public CourseLevel Level { get; private set; }
    public CourseStatus Status { get; private set; }
    public Money Price { get; private set; } = null!;
    public string? Country { get; private set; }

    private Course() { }

    private Course(Guid id, string title, string description, Guid instructorId, Guid categoryId, string? thumbnailUrl, CourseLevel level, CourseStatus status, Money price, bool isIncludedInSubscription, SubscriptionTier requiredSubscriptionTier, Language language, string? country) : base(id)
    {
        Title = title;
        Description = description;
        InstructorId = instructorId;
        CategoryId = categoryId;
        ThumbnailUrl = thumbnailUrl;
        Level = level;
        Status = status;
        Price = price;
        IsIncludedInSubscription = isIncludedInSubscription;
        RequiredSubscriptionTier = requiredSubscriptionTier;
        Language = language;
        Country = country;
    }

    public static Result<Course> Create(Guid id, string title, string description, Guid instructorId, Guid categoryId, string? thumbnailUrl, CourseLevel level, CourseStatus status, Money price, bool isIncludedInSubscription, SubscriptionTier requiredSubscriptionTier, string language, string? languageName, string? country)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return CourseErrors.TitleRequired;
        }
        if (string.IsNullOrWhiteSpace(description))
        {
            return CourseErrors.DescriptionRequired;
        }
        if (Guid.Empty.Equals(instructorId))
        {
            return CourseErrors.InstructorIdRequired;
        }
        if (categoryId == Guid.Empty)
        {
            return CourseClassificationErrors.CategoryIdRequired;
        }
        if (!Enum.IsDefined(typeof(CourseLevel), level))
        {
            return CourseErrors.InvalidCourseLevel;
        }
        if (!Enum.IsDefined(typeof(CourseStatus), status))
        {
            return CourseErrors.InvalidCourseStatus;
        }
        if (price is null)
        {
            return CourseErrors.PriceRequired;
        }
        var languageVoResult = LearnHub.Domain.Common.ValueObjects.Language.Create(language ?? string.Empty, languageName ?? string.Empty);
        if (languageVoResult.IsError)
        {
            return languageVoResult.Errors;
        }

        if (!Enum.IsDefined(typeof(SubscriptionTier), requiredSubscriptionTier))
        {
            return CourseErrors.InvalidSubscriptionTier;
        }

        return new Course(id, title, description, instructorId, categoryId, thumbnailUrl, level, status, price, isIncludedInSubscription, requiredSubscriptionTier, languageVoResult.Value, country);
    }

    public Result<Updated> Update(string title, string description, Guid categoryId, string? thumbnailUrl, CourseLevel level, CourseStatus status, Money price, bool isIncludedInSubscription, SubscriptionTier requiredSubscriptionTier, string language, string? languageName, string? country)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return CourseErrors.TitleRequired;
        }
        if (string.IsNullOrWhiteSpace(description))
        {
            return CourseErrors.DescriptionRequired;
        }
        if (categoryId == Guid.Empty)
        {
            return CourseClassificationErrors.CategoryIdRequired;
        }
        if (!Enum.IsDefined(typeof(CourseLevel), level))
        {
            return CourseErrors.InvalidCourseLevel;
        }
        if (!Enum.IsDefined(typeof(CourseStatus), status))
        {
            return CourseErrors.InvalidCourseStatus;
        }
        if (price is null)
        {
            return CourseErrors.PriceRequired;
        }
        var languageVoResult = LearnHub.Domain.Common.ValueObjects.Language.Create(language ?? string.Empty, languageName ?? string.Empty);
        if (languageVoResult.IsError)
        {
            return languageVoResult.Errors;
        }

        if (!Enum.IsDefined(typeof(SubscriptionTier), requiredSubscriptionTier))
        {
            return CourseErrors.InvalidSubscriptionTier;
        }

        Title = title;
        Description = description;
        CategoryId = categoryId;
        ThumbnailUrl = thumbnailUrl;
        Level = level;
        IsIncludedInSubscription = isIncludedInSubscription;
        RequiredSubscriptionTier = requiredSubscriptionTier;
        if (Status != status)
        {
            var statusChangeResult = ChangeStatus(status);
            if (statusChangeResult.IsError)
            {
                return statusChangeResult.Errors;
            }
        }
        Price = price;
        Language = languageVoResult.Value;
        Country = country;
        UpdatedAtUtc = DateTimeOffset.UtcNow;

        return Result.Updated;
    }

    public Result<Updated> ChangeCategory(Guid categoryId)
    {
        if (categoryId == Guid.Empty)
        {
            return CourseClassificationErrors.CategoryIdRequired;
        }

        if (CategoryId == categoryId)
        {
            return Result.Updated;
        }

        CategoryId = categoryId;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
        return Result.Updated;
    }

    public Result<Updated> AddTag(Guid tagId, int maxTags = 10)
    {
        if (tagId == Guid.Empty)
        {
            return CourseClassificationErrors.TagIdRequired;
        }

        if (_tagIds.Contains(tagId))
        {
            return CourseClassificationErrors.TagAlreadyAssigned;
        }

        if (_tagIds.Count >= maxTags)
        {
            return CourseClassificationErrors.TagLimitReached;
        }

        _tagIds.Add(tagId);
        UpdatedAtUtc = DateTimeOffset.UtcNow;

        return Result.Updated;
    }

    public Result<Updated> RemoveTag(Guid tagId)
    {
        if (tagId == Guid.Empty)
        {
            return CourseClassificationErrors.TagIdRequired;
        }

        if (!_tagIds.Remove(tagId))
        {
            return CourseClassificationErrors.TagNotFound;
        }

        UpdatedAtUtc = DateTimeOffset.UtcNow;
        return Result.Updated;
    }

    public Result<Updated> ChangeStatus(CourseStatus status)
    {
        if (!Enum.IsDefined(typeof(CourseStatus), status))
        {
            return CourseErrors.InvalidCourseStatus;
        }

        if (Status == status)
        {
            return Result.Updated;
        }

        var previousStatus = Status;
        Status = status;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
        AddDomainEvent(new CourseStatusChangedDomainEvent(Id, previousStatus, status));

        return Result.Updated;
    }

    public Result<Updated> UpsertSections(List<Section> upcomingSections)
    {
        if (upcomingSections is null)
        {
            return CourseErrors.SectionsRequired;
        }

        _sections.RemoveAll(existing => upcomingSections.All(s => s.Id != existing.Id));
        foreach (Section section in upcomingSections)
        {
            var existing = _sections.FirstOrDefault(v => v.Id == section.Id);
            if (existing is null)
            {
                _sections.Add(section);
            }
            else
            {
                var updateSectionResult = existing.Update(section.Title, section.Description, section.Order);

                if (updateSectionResult.IsError)
                {
                    return updateSectionResult.Errors;
                }

                var upsertLessonsResult = existing.UpsertLessons(section.Lessons.ToList());
                if (upsertLessonsResult.IsError)
                {
                    return upsertLessonsResult.Errors;
                }
            }
        }

        UpdatedAtUtc = DateTimeOffset.UtcNow;
        return Result.Updated;
    }

}
