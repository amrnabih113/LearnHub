namespace LearnHub.Application.Features.Courses.Dtos;

public sealed record CategoryDto(
    Guid Id,
    string Name,
    string Slug,
    string? Description);