namespace LearnHub.Contracts.Admin.Requests;

public sealed record CreateCategoryRequest(
    string Name,
    string Slug,
    string? Description = null,
    Guid? ParentCategoryId = null);
