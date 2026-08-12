using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Reviews;
using LearnHub.Domain.Reviews.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Reviews.Commands.ModerateCourseReview;

public sealed class ModerateCourseReviewCommandHandler(IAppDbContext context)
    : IRequestHandler<ModerateCourseReviewCommand, Result<Updated>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<Updated>> Handle(
        ModerateCourseReviewCommand request,
        CancellationToken cancellationToken)
    {
        var review = await _context.CourseReviews
            .FirstOrDefaultAsync(r => r.Id == request.ReviewId, cancellationToken);

        if (review is null)
        {
            return ReviewErrors.ReviewNotFound;
        }

        var result = request.TargetStatus switch
        {
            ReviewStatus.Published => review.Publish(),
            ReviewStatus.Flagged => review.Flag(),
            ReviewStatus.Hidden => review.Hide(),
            ReviewStatus.Removed => review.Remove(),
            _ => Result.Updated
        };

        if (result.IsError)
        {
            return result.Errors;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Updated;
    }
}
