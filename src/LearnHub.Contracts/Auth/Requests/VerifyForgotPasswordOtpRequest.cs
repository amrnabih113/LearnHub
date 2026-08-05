namespace LearnHub.Contracts.Auth.Requests;

public sealed record VerifyForgotPasswordOtpRequest(
    string Email,
    string Otp);
