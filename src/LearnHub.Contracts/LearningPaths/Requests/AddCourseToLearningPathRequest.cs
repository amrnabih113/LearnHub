namespace LearnHub.Contracts.LearningPaths.Requests;

public sealed record AddCourseToLearningPathRequest(
    Guid CourseId,
    int? TargetOrder = null,
    bool IsRequired = true);
