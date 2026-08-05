namespace LearnHub.Contracts.Auth.Requests;

public sealed record ResetPasswordRequest(
    string ResetToken,
    string NewPassword);
