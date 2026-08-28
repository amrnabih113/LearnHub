namespace LearnHub.Contracts.Courses.Requests;

public sealed record LessonOrderItemRequest(
    Guid LessonId,
    int Order);

public sealed record ReorderLessonsRequest(
    IReadOnlyList<LessonOrderItemRequest> Items);
