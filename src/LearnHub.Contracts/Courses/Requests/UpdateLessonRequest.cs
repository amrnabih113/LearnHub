namespace LearnHub.Contracts.Courses.Requests;

public sealed record UpdateLessonRequest(
    string Title,
    string Description,
    string VideoUrl,
    bool IsPreview,
    string Content,
    int DurationInMinutes,
    int Order);
