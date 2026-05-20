using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.Identity;

public static class UserErrors
{
    public static Error FirstNameRequired => Error.Validation(
        code: "DomainError.User.FirstNameRequired",
        description:
        "First name is required."
    );
    public static Error LastNameRequired => Error.Validation(
        code: "DomainError.User.LastNameRequired",
        description:
        "Last name is required."
    );
    public static Error EmailRequired => Error.Validation(
        code: "DomainError.User.EmailRequired",
        description:
        "Email is required."
    );
    public static Error PasswordHashRequired => Error.Validation(
        code: "DomainError.User.PasswordHashRequired",
        description:
        "Email is required."
    );
    public static Error InvalidEmail => Error.Validation(
        code: "DomainError.User.InvalidEmail",
        description:
        "Email is not valid."
    );

    public static Error PhoneNumberRequired => Error.Validation(
        code: "DomainError.User.PhoneNumberRequired",
        description:
        "Phone number is required."
    );
    public static Error InvalidPhoneNumber => Error.Validation(
        code: "DomainError.User.InvalidPhoneNumber",
        description:
        "Phone number is not valid."
    );
    public static Error InvalidRole => Error.Validation(
        code: "DomainError.User.InvalidRole",
        description:
        "Role is not valid."
    );
}