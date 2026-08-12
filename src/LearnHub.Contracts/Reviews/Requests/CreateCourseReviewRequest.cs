namespace LearnHub.Contracts.Reviews.Requests;

public sealed record CreateCourseReviewRequest(
    int Rating,
    string Comment);
