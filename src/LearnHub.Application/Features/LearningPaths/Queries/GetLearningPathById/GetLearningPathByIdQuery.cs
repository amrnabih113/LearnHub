using LearnHub.Application.Features.LearningPaths.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.LearningPaths.Queries.GetLearningPathById;

public sealed record GetLearningPathByIdQuery(Guid LearningPathId)
    : IRequest<Result<LearningPathDetailDto>>;
