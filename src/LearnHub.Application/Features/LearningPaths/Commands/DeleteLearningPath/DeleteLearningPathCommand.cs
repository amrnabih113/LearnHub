using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.LearningPaths.Commands.DeleteLearningPath;

public sealed record DeleteLearningPathCommand(Guid LearningPathId)
    : IRequest<Result<Deleted>>;
