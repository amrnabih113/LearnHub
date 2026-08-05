namespace LearnHub.Contracts.Auth.Requests;

public sealed record LoginRequest(
    string Email,
    string Password);
