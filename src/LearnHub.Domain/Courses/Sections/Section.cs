using LearnHub.Domain.Common;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Courses.Sections.Lessons;

namespace LearnHub.Domain.Courses.Sections;

public sealed class Section : AuditableEntity
{
    public string Title { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public int LessonCount => Lessons.Count();
    public int DurationInMinutes => Lessons.Sum(l => l.DurationInMinutes);
    public int Order { get; private set; }
    public bool IsPublished { get; private set; }
    public Guid CourseId { get; private set; }
    public Course Course { get; private set; } = default!;

    private readonly List<Lesson> _lessons = [];
    public IEnumerable<Lesson> Lessons => _lessons.AsReadOnly();

    private Section() { }

    private Section(Guid id, string title, string description, int order, Guid courseId, bool isPublished = true) : base(id)
    {
        Title = title;
        Description = description;
        Order = order;
        CourseId = courseId;
        IsPublished = isPublished;
    }

    public static Result<Section> Create(Guid id, string title, string description, int order, Guid courseId, bool isPublished = true)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return SectionErrors.TitleRequired;
        }
        if (order <= 0)
        {
            return SectionErrors.InvalidOrder;
        }
        if (courseId == Guid.Empty)
        {
            return SectionErrors.CourseIdRequired;
        }

        return new Section(id, title, description ?? string.Empty, order, courseId, isPublished);
    }

    public Result<Updated> Update(string title, string description, int order)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return SectionErrors.TitleRequired;
        }
        if (order <= 0)
        {
            return SectionErrors.InvalidOrder;
        }

        Title = title;
        Description = description ?? string.Empty;
        Order = order;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
        return Result.Updated;
    }

    public Result<Updated> UpdateOrder(int newOrder)
    {
        if (newOrder <= 0)
        {
            return SectionErrors.InvalidOrder;
        }

        Order = newOrder;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
        return Result.Updated;
    }

    public Result<Updated> Publish()
    {
        IsPublished = true;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
        return Result.Updated;
    }

    public Result<Updated> Unpublish()
    {
        IsPublished = false;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
        return Result.Updated;
    }

    public Result<Updated> UpsertLessons(List<Lesson> upcomingLessons)
    {
        if (upcomingLessons is null)
        {
            return SectionErrors.LessonsRequired;
        }

        _lessons.RemoveAll(existing => upcomingLessons.All(lesson => lesson.Id != existing.Id));

        foreach (var lesson in upcomingLessons)
        {
            var existing = _lessons.FirstOrDefault(current => current.Id == lesson.Id);
            if (existing is null)
            {
                _lessons.Add(lesson);
                continue;
            }

            var updateLessonResult = existing.Update(
                lesson.Title!,
                lesson.Description!,
                lesson.VideoUrl!,
                lesson.IsPreview,
                lesson.Content!,
                lesson.DurationInMinutes,
                lesson.Order);

            if (updateLessonResult.IsError)
            {
                return updateLessonResult.Errors;
            }

            var upsertResourcesResult = existing.UpsertResources(lesson.Resources.ToList());
            if (upsertResourcesResult.IsError)
            {
                return upsertResourcesResult.Errors;
            }
        }

        UpdatedAtUtc = DateTimeOffset.UtcNow;
        return Result.Updated;
    }
}