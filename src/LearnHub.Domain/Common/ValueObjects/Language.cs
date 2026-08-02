using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.Common.ValueObjects;

public sealed record Language
{
    public string Code { get; private set; } = default!;

    public string Name { get; private set; } = default!;


    private Language()
    {
        // EF Core
    }


    private Language(
        string code,
        string name)
    {
        Code = code;
        Name = name;
    }


    public static Result<Language> Create(
        string code,
        string name)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return LanguageErrors.CodeRequired;
        }


        if (string.IsNullOrWhiteSpace(name))
        {
            return LanguageErrors.NameRequired;
        }


        var normalizedCode = code
            .Trim()
            .ToLowerInvariant();


        if (normalizedCode.Length < 2 ||
            normalizedCode.Length > 5)
        {
            return LanguageErrors.InvalidCode;
        }


        return new Language(
            normalizedCode,
            name.Trim());
    }


    public override string ToString()
        => $"{Name} ({Code})";
}