namespace LearnHub.Contracts.Instructor.Requests;

public sealed record UpdateInstructorProfileRequest(
    string? ProfessionalTitle = null,
    string? Headline = null,
    string? Biography = null);
