using LearnHub.Domain.Common;

namespace LearnHub.Domain.Courses.Events;

public sealed class CourseThumbnailUpdatedDomainEvent(Guid courseId, string? oldThumbnailUrl, string? newThumbnailUrl) : DomainEvent
{
    public Guid CourseId { get; } = courseId;

    public string? OldThumbnailUrl { get; } = oldThumbnailUrl;

    public string? NewThumbnailUrl { get; } = newThumbnailUrl;
}