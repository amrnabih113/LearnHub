namespace LearnHub.Contracts.Admin.Responses;

public sealed record TagResponse(
    Guid Id,
    string Name,
    string Slug,
    string? Description,
    DateTimeOffset CreatedAtUtc);
