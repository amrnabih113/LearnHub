using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.Identity;

public static class PasswordResetTokenErrors
{
    public static Error IdRequired => Error.Validation(
        code: "DomainError.PasswordResetToken.IdRequired",
        description: "Password reset token ID is required");

    public static Error TokenRequired => Error.Validation(
        code: "DomainError.PasswordResetToken.TokenRequired",
        description: "Password reset token is required");

    public static Error UserIdRequired => Error.Validation(
        code: "DomainError.PasswordResetToken.UserIdRequired",
        description: "User id is required for password reset token");

    public static Error ExpiryInvalid => Error.Validation(
        code: "DomainError.PasswordResetToken.ExpiryInvalid",
        description: "Password reset token expiry must be in the future");
}