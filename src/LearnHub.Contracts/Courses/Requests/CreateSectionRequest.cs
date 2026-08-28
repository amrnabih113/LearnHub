namespace LearnHub.Contracts.Courses.Requests;

public sealed record CreateSectionRequest(
    string Title,
    string? Description = null,
    int? Order = null);
