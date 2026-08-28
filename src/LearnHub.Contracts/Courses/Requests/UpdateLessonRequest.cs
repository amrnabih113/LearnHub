namespace LearnHub.Contracts.Courses.Requests;

public sealed record UpdateLessonRequest(
    string Title,
    string? Description = null,
    string? Content = null,
    bool IsPreview = false);
