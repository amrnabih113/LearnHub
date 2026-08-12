using LearnHub.Application.Features.Reviews.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Reviews.Commands.UpdateCourseReview;

public sealed record UpdateCourseReviewCommand(
    Guid ReviewId,
    Guid StudentId,
    int Rating,
    string Comment) : IRequest<Result<CourseReviewDto>>;
