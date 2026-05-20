using LearnHub.Domain.Common.ValueObjects;

namespace LearnHub.Domain.Courses.Sections.Lessons;

public sealed record class SubtitleTrack
{
    public SubtitleTrack(Language language, string url, bool isDefault = false)
    {
        Language = language;
        Url = url;
        IsDefault = isDefault;
    }

    public Language Language { get; private set; }
    public string Url { get; private set; }
    public bool IsDefault { get; private set; }
}
