namespace LearnHub.Contracts.Courses.Requests;

public sealed record CreateLessonRequest(
    string Title,
    string? Description = null,
    string? Content = null,
    bool IsPreview = false,
    int? Order = null);
