namespace LearnHub.Contracts.Admin.Requests;

public sealed record UpdateTagRequest(
    string Name,
    string Slug,
    string? Description = null);
