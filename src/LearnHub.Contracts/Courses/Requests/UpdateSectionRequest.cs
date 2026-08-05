namespace LearnHub.Contracts.Courses.Requests;

public sealed record UpdateSectionRequest(
    string Title,
    string Description,
    int Order);
