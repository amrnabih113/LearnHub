using LearnHub.Application.common.Models;
using LearnHub.Application.Features.LearningPaths.Dtos;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Courses.Enums;
using LearnHub.Domain.LearningPaths.Enums;
using MediatR;

namespace LearnHub.Application.Features.LearningPaths.Queries.GetLearningPaths;

public sealed record GetLearningPathsQuery(
    string? Search = null,
    CourseLevel? Level = null,
    LearningPathStatus? Status = LearningPathStatus.Published,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<Result<PagedResult<LearningPathDto>>>;
