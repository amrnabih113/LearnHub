using LearnHub.Domain.Common;

using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Courses.Sections.Lessons;

namespace LearnHub.Domain.Courses.Sections;

public class Section : AuditableEntity
{
    public string Title { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public int LessonCount => Lessons.Count();
    public int DurationInMinutes => Lessons.Sum(l => l.DurationInMinutes);
    public int Order { get; private set; }
    public Guid CourseId { get; private set; }
    public Course Course { get; private set; } = default!;

    public readonly List<Lesson> _lessons = [];
    public IEnumerable<Lesson> Lessons => _lessons.AsReadOnly();

    private Section() { }

    private Section(Guid id, string title, string description, int order, Guid courseId) : base(id)
    {
        Title = title;
        Description = description;
        Order = order;
        CourseId = courseId;
    }

    public static Result<Section> Create(Guid id, string title, string description, int order, Guid courseId)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return SectionErrors.TitleRequired;
        }
        if (string.IsNullOrWhiteSpace(description))
        {
            return SectionErrors.DescriptionRequired;
        }
        if (order <= 0)
        {
            return SectionErrors.InvalidOrder;
        }
        if (courseId == Guid.Empty)
        {
            return SectionErrors.CourseIdRequired;
        }

        return new Section(id, title, description, order, courseId);
    }

    public Result<Updated> Update(string title, string description, int order)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return SectionErrors.TitleRequired;
        }
        if (string.IsNullOrWhiteSpace(description))
        {
            return SectionErrors.DescriptionRequired;
        }
        if (order <= 0)
        {
            return SectionErrors.InvalidOrder;
        }

        Title = title;
        Description = description;
        Order = order;
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