using LearnHub.Application.common.Models;
using LearnHub.Application.Features.Reviews.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Reviews.Queries.GetCourseReviews;

public sealed record GetCourseReviewsQuery(
    Guid CourseId,
    int PageNumber = 1,
    int PageSize = 10) : IRequest<Result<PagedResult<CourseReviewDto>>>;
