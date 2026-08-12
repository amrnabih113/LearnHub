namespace LearnHub.Application.Features.Admin.Dtos;

public sealed record CategoryAdminDto(
    Guid Id,
    string Name,
    string Slug,
    string? Description,
    Guid? ParentCategoryId,
    string? ParentCategoryName,
    DateTimeOffset CreatedAtUtc);
