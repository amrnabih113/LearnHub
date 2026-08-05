namespace LearnHub.Application.Features.Courses.Dtos;

public sealed record TagDto(
    Guid Id,
    string Name,
    string Slug,
    string? Description);