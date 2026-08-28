namespace LearnHub.Contracts.LearningPaths.Requests;

public sealed record ReorderLearningPathCoursesRequest(
    List<Guid> OrderedCourseIds);
