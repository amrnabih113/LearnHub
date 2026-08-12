using LearnHub.Application.Features.Reviews.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Reviews.Queries.GetCourseReviewSummary;

public sealed record GetCourseReviewSummaryQuery(Guid CourseId) : IRequest<Result<ReviewSummaryDto>>;
