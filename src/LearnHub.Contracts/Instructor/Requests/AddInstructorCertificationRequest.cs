namespace LearnHub.Contracts.Instructor.Requests;

public sealed record AddInstructorCertificationRequest(
    string Title,
    string Issuer,
    DateOnly IssueDate,
    DateOnly? ExpirationDate,
    string? CredentialUrl);
