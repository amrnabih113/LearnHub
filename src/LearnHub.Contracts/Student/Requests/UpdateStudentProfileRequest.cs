namespace LearnHub.Contracts.Student.Requests;

public sealed record UpdateStudentProfileRequest(
    string FirstName,
    string LastName,
    string? PhoneNumber,
    DateOnly? DateOfBirth,
    string? Bio,
    string? Country);
