namespace LearnHub.Application.Features.Admin.Dtos;

public sealed record TagAdminDto(
    Guid Id,
    string Name,
    string Slug,
    string? Description,
    DateTimeOffset CreatedAtUtc);
