using LearnHub.Application.Features.Reviews.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Reviews.Queries.GetStudentCourseReview;

public sealed record GetStudentCourseReviewQuery(
    Guid CourseId,
    Guid StudentId) : IRequest<Result<CourseReviewDto?>>;
