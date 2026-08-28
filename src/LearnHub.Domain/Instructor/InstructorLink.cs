using LearnHub.Domain.Common;
using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.Instructor;

public sealed class InstructorLink : AuditableEntity
{
    public Guid InstructorProfileId { get; private set; }
    public string Title { get; private set; } = default!;
    public string Url { get; private set; } = default!;

    private InstructorLink() { }

    private InstructorLink(
        Guid id,
        Guid instructorProfileId,
        string title,
        string url) : base(id)
    {
        InstructorProfileId = instructorProfileId;
        Title = title;
        Url = url;
    }

    public static Result<InstructorLink> Create(
        Guid id,
        Guid instructorProfileId,
        string title,
        string url)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return Error.Validation("InstructorLink.TitleRequired", "Link title is required.");
        }
        if (string.IsNullOrWhiteSpace(url))
        {
            return Error.Validation("InstructorLink.UrlRequired", "URL is required.");
        }

        if (!Uri.TryCreate(url.Trim(), UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            return Error.Validation("InstructorLink.InvalidUrlScheme", "URL must be a valid HTTP or HTTPS address.");
        }

        return new InstructorLink(id, instructorProfileId, title.Trim(), url.Trim());
    }
}
