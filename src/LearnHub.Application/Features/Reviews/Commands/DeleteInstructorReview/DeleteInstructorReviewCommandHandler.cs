using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Reviews;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Reviews.Commands.DeleteInstructorReview;

public sealed class DeleteInstructorReviewCommandHandler(IAppDbContext context)
    : IRequestHandler<DeleteInstructorReviewCommand, Result<Deleted>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<Deleted>> Handle(
        DeleteInstructorReviewCommand request,
        CancellationToken cancellationToken)
    {
        var review = await _context.InstructorReviews
            .FirstOrDefaultAsync(r => r.Id == request.ReviewId, cancellationToken);

        if (review is null)
        {
            return ReviewErrors.ReviewNotFound;
        }

        if (review.StudentId != request.StudentId && !request.IsAdminOrInstructor)
        {
            return ReviewErrors.UnauthorizedToModifyReview;
        }

        _context.InstructorReviews.Remove(review);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Deleted;
    }
}
