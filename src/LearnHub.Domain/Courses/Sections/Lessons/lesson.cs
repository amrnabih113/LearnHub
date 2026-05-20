using LearnHub.Domain.Common;

using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Courses.Sections.Lessons.Resources;

namespace LearnHub.Domain.Courses.Sections.Lessons;

public class Lesson : AuditableEntity
{
    public string? Title { get; private set; }
    public string? Description { get; private set; }
    public string VideoUrl { get; private set; } = string.Empty;
    public bool IsPreview { get; private set; }
    public readonly List<Resource> _resources = [];
    public IEnumerable<Resource> Resources => _resources.AsReadOnly();
    private readonly List<SubtitleTrack> _subtitles = [];
    public IReadOnlyCollection<SubtitleTrack> Subtitles => _subtitles.AsReadOnly();
    private readonly List<VideoQuality> _qualities = [];
    public IReadOnlyCollection<VideoQuality> Qualities => _qualities.AsReadOnly();
    public string? Content { get; private set; }
    public int DurationInMinutes { get; private set; }
    public int Order { get; private set; }

    public Guid SectionId { get; private set; }
    public Section? Section { get; private set; }

    private Lesson() { }

    private Lesson(Guid id,
                   string title,
                   string description,
                   string videoUrl,
                   bool isPreview,
                   string content,
                   int durationInMinutes,
                   int order,
                   Guid sectionId) : base(id)
    {
        Title = title;
        Description = description;
        VideoUrl = videoUrl;
        IsPreview = isPreview;
        Content = content;
        DurationInMinutes = durationInMinutes;
        Order = order;
        SectionId = sectionId;
    }

    public static Result<Lesson> Create(Guid id, string title, string description, string videoUrl, bool isPreview, string content, int durationInMinutes, int order, Guid sectionId)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return LessonErrors.TitleRequired;
        }
        if (string.IsNullOrWhiteSpace(description))
        {
            return LessonErrors.DescriptionRequired;
        }
        if (string.IsNullOrWhiteSpace(videoUrl))
        {
            return LessonErrors.VideoUrlRequired;
        }
        if (string.IsNullOrWhiteSpace(content))
        {
            return LessonErrors.ContentRequired;
        }
        if (durationInMinutes <= 0)
        {
            return LessonErrors.InvalidDuration;
        }
        if (order <= 0)
        {
            return LessonErrors.InvalidOrder;
        }
        if (sectionId == Guid.Empty)
        {
            return LessonErrors.SectionIdRequired;
        }

        return new Lesson(id, title, description, videoUrl, isPreview, content, durationInMinutes, order, sectionId);
    }

    public Result<Updated> Update(string title, string description, string videoUrl, bool isPreview, string content, int durationInMinutes, int order)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return LessonErrors.TitleRequired;
        }
        if (string.IsNullOrWhiteSpace(description))
        {
            return LessonErrors.DescriptionRequired;
        }
        if (string.IsNullOrWhiteSpace(videoUrl))
        {
            return LessonErrors.VideoUrlRequired;
        }
        if (string.IsNullOrWhiteSpace(content))
        {
            return LessonErrors.ContentRequired;
        }
        if (durationInMinutes <= 0)
        {
            return LessonErrors.InvalidDuration;
        }
        if (order <= 0)
        {
            return LessonErrors.InvalidOrder;
        }

        Title = title;
        Description = description;
        VideoUrl = videoUrl;
        IsPreview = isPreview;
        Content = content;
        DurationInMinutes = durationInMinutes;
        Order = order;
        UpdatedAtUtc = DateTimeOffset.UtcNow;

        return Result.Updated;
    }

    public Result<Updated> UpsertResources(List<Resource>? upcomingResources)
    {
        if (upcomingResources is null)
        {
            return Result.Updated;
        }

        _resources.RemoveAll(existing => upcomingResources.All(resource => resource.Id != existing.Id));

        foreach (var resource in upcomingResources)
        {
            var existing = _resources.FirstOrDefault(current => current.Id == resource.Id);
            if (existing is null)
            {
                _resources.Add(resource);
                continue;
            }

            var updateResourceResult = existing.Update(resource.Name, resource.Url, resource.Type, resource.SizeInBytes);
            if (updateResourceResult.IsError)
            {
                return updateResourceResult.Errors;
            }
        }

        UpdatedAtUtc = DateTimeOffset.UtcNow;
        return Result.Updated;
    }
}