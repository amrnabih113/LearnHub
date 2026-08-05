using LearnHub.Domain.Courses.Sections.Lessons.Resources;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Courses.Commands.CreateResource;

public sealed record CreateResourceCommand(
    Guid LessonId,
    string Name,
    string Url,
    ResourceType Type,
    long SizeInBytes) : IRequest<Result<Guid>>;