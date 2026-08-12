namespace LearnHub.Contracts.Reviews.Requests;

public sealed record UpdateInstructorReviewRequest(
    int Rating,
    string Comment);
