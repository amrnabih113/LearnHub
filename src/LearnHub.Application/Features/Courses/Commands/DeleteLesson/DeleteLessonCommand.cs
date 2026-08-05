using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Courses.Commands.DeleteLesson;

public sealed record DeleteLessonCommand(Guid LessonId) : IRequest<Result<Deleted>>;