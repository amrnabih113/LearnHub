using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Courses.Commands.CreateLesson;

public sealed record CreateLessonCommand(
    Guid SectionId,
    string Title,
    string Description,
    string VideoUrl,
    bool IsPreview,
    string Content,
    int DurationInMinutes,
    int Order) : IRequest<Result<Guid>>;