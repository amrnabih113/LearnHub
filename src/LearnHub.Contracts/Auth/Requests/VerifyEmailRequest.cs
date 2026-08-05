namespace LearnHub.Contracts.Auth.Requests;

public sealed record VerifyEmailRequest(
    string Email,
    string Otp);
