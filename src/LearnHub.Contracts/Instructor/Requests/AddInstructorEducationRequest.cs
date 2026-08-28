namespace LearnHub.Contracts.Instructor.Requests;

public sealed record AddInstructorEducationRequest(
    string Institution,
    string Degree,
    string FieldOfStudy,
    DateOnly StartDate,
    DateOnly? EndDate,
    string? Description);
