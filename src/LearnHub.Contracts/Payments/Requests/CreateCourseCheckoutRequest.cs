namespace LearnHub.Contracts.Payments.Requests;

public sealed record CreateCourseCheckoutRequest(
    Guid CourseId,
    string SuccessUrl,
    string CancelUrl);
