namespace LearnHub.Contracts.Courses.Requests;

public sealed record CreateLessonRequest(
    string Title,
    string Description,
    string VideoUrl,
    bool IsPreview,
    string Content,
    int DurationInMinutes,
    int Order);
