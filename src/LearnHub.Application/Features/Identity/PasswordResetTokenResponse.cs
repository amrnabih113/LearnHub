namespace LearnHub.Application.Features.Identity;

public sealed record PasswordResetTokenResponse(
    string ResetToken,
    DateTimeOffset ExpiresOnUtc);