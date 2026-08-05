using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Courses.Commands.UpdateLesson;

public sealed record UpdateLessonCommand(
    Guid LessonId,
    string Title,
    string Description,
    string VideoUrl,
    bool IsPreview,
    string Content,
    int DurationInMinutes,
    int Order) : IRequest<Result<Updated>>;