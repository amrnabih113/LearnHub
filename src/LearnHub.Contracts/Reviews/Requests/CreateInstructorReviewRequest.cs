namespace LearnHub.Contracts.Reviews.Requests;

public sealed record CreateInstructorReviewRequest(
    int Rating,
    string Comment,
    Guid? CourseId = null);
