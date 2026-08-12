using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Reviews.Commands.DeleteInstructorReview;

public sealed record DeleteInstructorReviewCommand(
    Guid ReviewId,
    Guid StudentId,
    bool IsAdminOrInstructor = false) : IRequest<Result<Deleted>>;
