using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.LearningPaths.Commands.PublishLearningPath;

public sealed record PublishLearningPathCommand(Guid LearningPathId)
    : IRequest<Result<Updated>>;
