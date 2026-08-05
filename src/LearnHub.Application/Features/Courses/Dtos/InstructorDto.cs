namespace LearnHub.Application.Features.Courses.Dtos;

public sealed record InstructorDto(
    Guid Id,
    string FirstName,
    string LastName,
    string FullName,
    string Email,
    string? ImageUrl);