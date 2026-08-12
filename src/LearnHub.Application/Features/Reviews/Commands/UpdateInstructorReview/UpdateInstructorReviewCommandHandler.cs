using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Reviews.Dtos;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Reviews;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Reviews.Commands.UpdateInstructorReview;

public sealed class UpdateInstructorReviewCommandHandler(IAppDbContext context)
    : IRequestHandler<UpdateInstructorReviewCommand, Result<InstructorReviewDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<InstructorReviewDto>> Handle(
        UpdateInstructorReviewCommand request,
        CancellationToken cancellationToken)
    {
        var review = await _context.InstructorReviews
            .FirstOrDefaultAsync(r => r.Id == request.ReviewId, cancellationToken);

        if (review is null)
        {
            return ReviewErrors.ReviewNotFound;
        }

        if (review.StudentId != request.StudentId)
        {
            return ReviewErrors.UnauthorizedToModifyReview;
        }

        var updateResult = review.Update(request.Rating, request.Comment);
        if (updateResult.IsError)
        {
            return updateResult.Errors;
        }

        await _context.SaveChangesAsync(cancellationToken);

        var student = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == review.StudentId, cancellationToken);

        return new InstructorReviewDto(
            Id: review.Id,
            InstructorId: review.InstructorId,
            StudentId: review.StudentId,
            StudentName: student?.FullName ?? string.Empty,
            StudentImageUrl: student?.ImageUrl,
            CourseId: review.CourseId,
            Rating: review.Rating.Value,
            Comment: review.Comment,
            Status: review.Status.ToString(),
            CreatedAtUtc: review.CreatedAtUtc);
    }
}
