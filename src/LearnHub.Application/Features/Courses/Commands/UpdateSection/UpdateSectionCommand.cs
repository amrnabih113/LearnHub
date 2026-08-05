using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Courses.Commands.UpdateSection;

public sealed record UpdateSectionCommand(
    Guid SectionId,
    string Title,
    string Description,
    int Order) : IRequest<Result<Updated>>;