using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Reviews.Commands.DeleteCourseReview;

public sealed record DeleteCourseReviewCommand(
    Guid ReviewId,
    Guid StudentId,
    bool IsAdminOrInstructor = false) : IRequest<Result<Deleted>>;
