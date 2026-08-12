using LearnHub.Application.Features.Reviews.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Reviews.Commands.CreateCourseReview;

public sealed record CreateCourseReviewCommand(
    Guid CourseId,
    Guid StudentId,
    int Rating,
    string Comment) : IRequest<Result<CourseReviewDto>>;
