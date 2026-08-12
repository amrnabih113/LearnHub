namespace LearnHub.Contracts.Admin.Responses;

public sealed record CategoryResponse(
    Guid Id,
    string Name,
    string Slug,
    string? Description,
    Guid? ParentCategoryId,
    string? ParentCategoryName,
    DateTimeOffset CreatedAtUtc);
