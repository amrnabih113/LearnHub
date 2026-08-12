namespace LearnHub.Contracts.Admin.Requests;

public sealed record CreateTagRequest(
    string Name,
    string Slug,
    string? Description = null);
