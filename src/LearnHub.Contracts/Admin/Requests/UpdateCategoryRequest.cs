namespace LearnHub.Contracts.Admin.Requests;

public sealed record UpdateCategoryRequest(
    string Name,
    string Slug,
    string? Description = null,
    Guid? ParentCategoryId = null);
