namespace LearnHub.Contracts.Instructor.Requests;

public sealed record AddInstructorLinkRequest(
    string Title,
    string Url);
