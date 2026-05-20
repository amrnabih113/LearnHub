using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.Identity;

public static class OtpCodeErrors
{
    public static Error CodeRequired
        => Error.Validation(code: "DomainError.OtpCode.CodeRequired", description: "OTP code is required");

    public static Error ExpirationMustBeInTheFuture
        => Error.Validation(code: "DomainError.OtpCode.ExpirationMustBeInTheFuture", description: "OTP expiration must be in the future");

    public static Error InvalidPurpose
        => Error.Validation(code: "DomainError.OtpCode.InvalidPurpose", description: "OTP purpose is invalid");

    public static Error UserIdRequired
        => Error.Validation(code: "DomainError.OtpCode.UserIdRequired", description: "User id is required for OTP code");
}
