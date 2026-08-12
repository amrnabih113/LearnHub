namespace LearnHub.Contracts.Reviews.Requests;

public sealed record UpdateCourseReviewRequest(
    int Rating,
    string Comment);
