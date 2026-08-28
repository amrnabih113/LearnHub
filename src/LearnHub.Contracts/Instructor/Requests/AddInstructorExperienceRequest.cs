namespace LearnHub.Contracts.Instructor.Requests;

public sealed record AddInstructorExperienceRequest(
    string Title,
    string Company,
    string? Description,
    DateOnly StartDate,
    DateOnly? EndDate,
    bool IsCurrent,
    string? Location);
