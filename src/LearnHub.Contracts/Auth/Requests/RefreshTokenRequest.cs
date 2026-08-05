namespace LearnHub.Contracts.Auth.Requests;

public sealed record RefreshTokenRequest(
    string ExpiredToken,
    string RefreshToken);
