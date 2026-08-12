using System.Security.Claims;
using LearnHub.Domain.Identity;

namespace LearnHub.Application.Features.Identity.Dtos;

public sealed record UserDto(
    Guid Id,
    string FirstName,
    string LastName,
    string FullName,
    string Email,
    string PhoneNumber,
    IReadOnlyCollection<string> Roles,
    string? ImageUrl = null,
    DateOnly? DateOfBirth = null,
    string? Bio = null,
    string? Country = null
);