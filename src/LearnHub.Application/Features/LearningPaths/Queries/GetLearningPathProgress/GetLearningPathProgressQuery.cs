using LearnHub.Application.Features.LearningPaths.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.LearningPaths.Queries.GetLearningPathProgress;

public sealed record GetLearningPathProgressQuery(
    Guid LearningPathId,
    Guid StudentId
) : IRequest<Result<LearningPathProgressDto>>;
