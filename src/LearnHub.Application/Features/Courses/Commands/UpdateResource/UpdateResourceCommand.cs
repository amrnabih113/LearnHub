using LearnHub.Domain.Courses.Sections.Lessons.Resources;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Courses.Commands.UpdateResource;

public sealed record UpdateResourceCommand(
    Guid ResourceId,
    string Name,
    string Url,
    ResourceType Type,
    long SizeInBytes) : IRequest<Result<Updated>>;