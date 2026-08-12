using LearnHub.Application.common.Models;
using LearnHub.Application.Features.Reviews.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Admin.Queries.GetReviewsAdmin;

public sealed record GetReviewsAdminQuery(
    Guid? CourseId = null,
    Guid? StudentId = null,
    int? Rating = null,
    string? Status = null,
    int PageNumber = 1,
    int PageSize = 10) : IRequest<Result<PagedResult<CourseReviewDto>>>;
