using LearnHub.Domain.Common;

using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Identity;
using LearnHub.Domain.Courses.Events;
using LearnHub.Domain.Courses.Enums;
using LearnHub.Domain.Courses.Tags;
using LearnHub.Domain.Courses.Sections;

namespace LearnHub.Domain.Courses;

public sealed class Course : AuditableEntity
{
    public string? Title { get; private set; }
    public string? Description { get; private set; }
    public string? InstructorId { get; private set; }
    public User? Instructor { get; private set; }

    private readonly List<Tag> _tags = [];
    public IEnumerable<Tag> Tags => _tags.AsReadOnly();

    private readonly List<Section> _sections = [];
    public IEnumerable<Section> Sections => _sections.AsReadOnly();
    public string? ThumbnailUrl { get; private set; }
    public CourseLevel Level { get; private set; }
    public CourseStatus Status { get; private set; }
    public decimal Price { get; private set; }
    public string? Country { get; private set; }

    private Course() { }

    private Course(Guid id, string title, string description, string instructorId, string? thumbnailUrl, CourseLevel level, CourseStatus status, decimal price, string? country) : base(id)
    {
        Title = title;
        Description = description;
        InstructorId = instructorId;
        ThumbnailUrl = thumbnailUrl;
        Level = level;
        Status = status;
        Price = price;
        Country = country;
    }

    public static Result<Course> Create(Guid id, string title, string description, string instructorId, string? thumbnailUrl, CourseLevel level, CourseStatus status, decimal price, string? country)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return CourseErrors.TitleRequired;
        }
        if (string.IsNullOrWhiteSpace(description))
        {
            return CourseErrors.DescriptionRequired;
        }
        if (string.IsNullOrWhiteSpace(instructorId))
        {
            return CourseErrors.InstructorIdRequired;
        }
        if (!Enum.IsDefined(typeof(CourseLevel), level))
        {
            return CourseErrors.InvalidCourseLevel;
        }
        if (!Enum.IsDefined(typeof(CourseStatus), status))
        {
            return CourseErrors.InvalidCourseStatus;
        }
        if (price < 0)
        {
            return CourseErrors.PriceRequired;
        }

        return new Course(id, title, description, instructorId, thumbnailUrl, level, status, price, country);
    }

    public Result<Updated> Update(string title, string description, string? thumbnailUrl, CourseLevel level, CourseStatus status, decimal price, string? country)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return CourseErrors.TitleRequired;
        }
        if (string.IsNullOrWhiteSpace(description))
        {
            return CourseErrors.DescriptionRequired;
        }
        if (!Enum.IsDefined(typeof(CourseLevel), level))
        {
            return CourseErrors.InvalidCourseLevel;
        }
        if (!Enum.IsDefined(typeof(CourseStatus), status))
        {
            return CourseErrors.InvalidCourseStatus;
        }
        if (price < 0)
        {
            return CourseErrors.PriceRequired;
        }

        Title = title;
        Description = description;
        ThumbnailUrl = thumbnailUrl;
        Level = level;
        if (Status != status)
        {
            var statusChangeResult = ChangeStatus(status);
            if (statusChangeResult.IsError)
            {
                return statusChangeResult.Errors;
            }
        }
        Price = price;
        Country = country;
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
