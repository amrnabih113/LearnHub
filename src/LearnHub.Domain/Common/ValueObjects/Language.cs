using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Common;

namespace LearnHub.Domain.Common.ValueObjects;


public sealed record Language
{
    public string Code { get; init; }

    private Language(string code)
    {
        Code = code;
    }

    public static Result<Language> Create(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return LanguageErrors.CodeRequired;
        }

        var normalized = code.Trim().ToLowerInvariant();
        if (normalized.Length < 2 || normalized.Length > 5)
        {
            return LanguageErrors.InvalidCode;
        }

        return new Language(normalized);
    }

    public override string ToString() => Code;
}
