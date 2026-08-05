using LearnHub.Domain.Identity;

namespace LearnHub.Contracts.Auth.Requests;

public sealed record RegisterRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string ConfirmPassword,
    Role Role,
    string? PhoneNumber = null);
