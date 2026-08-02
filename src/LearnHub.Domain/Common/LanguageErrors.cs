using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.Common;

public static class LanguageErrors
{
    public static Error CodeRequired
        => Error.Validation(code: "DomainError.Language.CodeRequired", description: "Language code is required");

    public static Error NameRequired
        => Error.Validation(code: "DomainError.Language.NameRequired", description: "Language name is required");
    public static Error InvalidCode
        => Error.Validation(code: "DomainError.Language.InvalidCode", description: "Language code must be 2-5 characters (e.g. 'en', 'fr', 'zh-CN')");
}
