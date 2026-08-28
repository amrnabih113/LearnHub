using LearnHub.Domain.Common;
using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.Instructor;

public sealed class InstructorLanguage : AuditableEntity
{
    public Guid InstructorProfileId { get; private set; }
    public string LanguageCode { get; private set; } = default!;
    public string LanguageName { get; private set; } = default!;

    private InstructorLanguage() { }

    private InstructorLanguage(Guid id, Guid instructorProfileId, string languageCode, string languageName) : base(id)
    {
        InstructorProfileId = instructorProfileId;
        LanguageCode = languageCode;
        LanguageName = languageName;
    }

    public static Result<InstructorLanguage> Create(Guid id, Guid instructorProfileId, string languageCode, string languageName)
    {
        if (string.IsNullOrWhiteSpace(languageCode))
        {
            return Error.Validation("InstructorLanguage.CodeRequired", "Language code is required.");
        }
        if (string.IsNullOrWhiteSpace(languageName))
        {
            return Error.Validation("InstructorLanguage.NameRequired", "Language name is required.");
        }

        return new InstructorLanguage(id, instructorProfileId, languageCode.Trim(), languageName.Trim());
    }
}
