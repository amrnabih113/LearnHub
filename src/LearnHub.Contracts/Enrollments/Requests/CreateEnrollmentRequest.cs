namespace LearnHub.Contracts.Enrollments.Requests;

public sealed record CreateEnrollmentRequest(
    Guid StudentId,
    Guid CourseId);
