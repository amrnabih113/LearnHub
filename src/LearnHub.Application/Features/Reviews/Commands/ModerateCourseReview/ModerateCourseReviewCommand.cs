using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Reviews.Enums;
using MediatR;

namespace LearnHub.Application.Features.Reviews.Commands.ModerateCourseReview;

public sealed record ModerateCourseReviewCommand(
    Guid ReviewId,
    ReviewStatus TargetStatus) : IRequest<Result<Updated>>;
