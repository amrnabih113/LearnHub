
using LearnHub.Domain.Common.Results;

namespace LearnHub.Application.common.Errors;

public static class ApplicationErrors
{
    public static Error InvalidCredentials => Error.Validation(
      code: "ApplicationError.User.InvalidCredentials",
      description:
      "Invalid email or password."
  );
    public static Error EmailNotVerified => Error.Validation(
        code: "ApplicationError.User.EmailNotVerified",
        description:
        "Email is not verified."
    );

    public static Error EmailAlreadyVerified  => Error.Validation(
        code: "ApplicationError.User.EmailAlreadyVerified",
        description:
        "Email is already verified."
    );

    public static Error InvalidRefreshToken => Error.Validation(
        code: "ApplicationError.User.InvalidRefreshToken",
        description:
        "Invalid refresh token."
    );

    public static Error RefreshTokenExpired => Error.Validation(
        code: "ApplicationError.User.ExpiredToken",
        description:
        "Refresh token has expired."
    );

    public static Error EmailAlreadyExists => Error.Validation(
        code: "ApplicationError.User.EmailAlreadyExists",
        description:
        "A user with the given email already exists."
    );
    public static Error PasswordsDontMatch => Error.Validation(
        code: "ApplicationError.User.PasswordsDontMatch",
        description:
        "The passwords do not match."
    );
    public static Error InvalidOldPassword => Error.Validation(
        code: "ApplicationError.User.InvalidOldPassword",
        description:
        "The old password is incorrect."
    );
    public static Error InvalidOtp => Error.Validation(
        code: "ApplicationError.User.InvalidOtp",
        description:
        "Invalid OTP."
    );
    public static Error OtpExpired => Error.Validation(
        code: "ApplicationError.User.ExpiredOtp",
        description:
        "OTP has expired."
    );

    public static Error UserNotFound => Error.Validation(
        code: "ApplicationError.User.NotFound",
        description:
        "User not found."
    );
    public static Error InvalidRole => Error.Validation(
        code: "ApplicationError.User.InvalidRole",
        description:
        "Invalid user role."
    );
      public static Error AdminRoleUnauthorized => Error.Validation(
        code: "ApplicationError.User.AdminRoleUnauthorized",
        description:
        "Only can register as a student or instructor. Admin role is not allowed."
    );
}
