using LearnHub.Domain.Common;

using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Courses.Sections.Lessons.Resources;

namespace LearnHub.Domain.Courses.Sections.Lessons;

public sealed class Lesson : AuditableEntity
{
    public string? Title { get; private set; }
    public string? Description { get; private set; }
    public string VideoUrl { get; private set; } = string.Empty;
    public bool IsPreview { get; private set; }
    private readonly List<Resource> _resources = [];
    public IEnumerable<Resource> Resources => _resources.AsReadOnly();
    private readonly List<SubtitleTrack> _subtitles = [];
    public IReadOnlyCollection<SubtitleTrack> Subtitles => _subtitles.AsReadOnly();
    private readonly List<VideoQuality> _qualities = [];
    public IReadOnlyCollection<VideoQuality> Qualities => _qualities.AsReadOnly();
    public string? Content { get; private set; }
    public int DurationInMinutes { get; private set; }
    public int Order { get; private set; }
    public bool IsPublished { get; private set; }

    public Guid SectionId { get; private set; }
    public Section Section { get; private set; } = default!;

    private Lesson() { }

    private Lesson(Guid id,
                   string title,
                   string description,
                   string videoUrl,
                   bool isPreview,
                   string content,
                   int durationInMinutes,
                   int order,
                   Guid sectionId,
                   bool isPublished = true) : base(id)
    {
        Title = title;
        Description = description;
        VideoUrl = videoUrl;
        IsPreview = isPreview;
        Content = content;
        DurationInMinutes = durationInMinutes;
        Order = order;
        SectionId = sectionId;
        IsPublished = isPublished;
    }

    public static Result<Lesson> Create(Guid id, string title, string? description, string? videoUrl, bool isPreview, string? content, int durationInMinutes, int order, Guid sectionId, bool isPublished = true)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return LessonErrors.TitleRequired;
        }
        if (order <= 0)
        {
            return LessonErrors.InvalidOrder;
        }
        if (sectionId == Guid.Empty)
        {
            return LessonErrors.SectionIdRequired;
        }

        return new Lesson(id, title.Trim(), description?.Trim() ?? string.Empty, videoUrl?.Trim() ?? string.Empty, isPreview, content?.Trim() ?? string.Empty, durationInMinutes, order, sectionId, isPublished);
    }

    public Result<Updated> Update(string title, string? description, string? videoUrl, bool isPreview, string? content, int durationInMinutes, int order)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return LessonErrors.TitleRequired;
        }
        if (order <= 0)
        {
            return LessonErrors.InvalidOrder;
        }

        Title = title.Trim();
        Description = description?.Trim() ?? string.Empty;
        if (videoUrl != null) VideoUrl = videoUrl.Trim();
        IsPreview = isPreview;
        Content = content?.Trim() ?? string.Empty;
        if (durationInMinutes > 0) DurationInMinutes = durationInMinutes;
        Order = order;
        UpdatedAtUtc = DateTimeOffset.UtcNow;

        return Result.Updated;
    }

    public Result<Updated> UpdateVideo(string videoUrl, int durationInMinutes)
    {
        if (string.IsNullOrWhiteSpace(videoUrl))
        {
            return LessonErrors.VideoUrlRequired;
        }
        if (durationInMinutes <= 0)
        {
            return LessonErrors.InvalidDuration;
        }

        VideoUrl = videoUrl.Trim();
        DurationInMinutes = durationInMinutes;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
        return Result.Updated;
    }

    public Result<Updated> UpdateOrder(int newOrder)
    {
        if (newOrder <= 0)
        {
            return LessonErrors.InvalidOrder;
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