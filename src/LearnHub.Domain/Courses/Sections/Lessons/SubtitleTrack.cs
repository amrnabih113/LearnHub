using LearnHub.Domain.Common;
using LearnHub.Domain.Common.ValueObjects;

namespace LearnHub.Domain.Courses.Sections.Lessons;

public sealed class SubtitleTrack : AuditableEntity
{

    public Language Language { get; private set; } = default!;
    public string Url { get; private set; } = default!;
    public bool IsDefault { get; private set; }
    private SubtitleTrack()
    {
    }
    public SubtitleTrack(Language language, string url, bool isDefault = false)
    {
        Language = language;
        Url = url;
        IsDefault = isDefault;
    }


}
