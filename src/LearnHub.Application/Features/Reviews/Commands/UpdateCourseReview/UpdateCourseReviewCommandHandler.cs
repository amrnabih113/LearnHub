using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Reviews.Dtos;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Reviews;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Reviews.Commands.UpdateCourseReview;

public sealed class UpdateCourseReviewCommandHandler(IAppDbContext context)
    : IRequestHandler<UpdateCourseReviewCommand, Result<CourseReviewDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<CourseReviewDto>> Handle(
        UpdateCourseReviewCommand request,
        CancellationToken cancellationToken)
    {
        var review = await _context.CourseReviews
            .Include(r => r.Student)
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

        return new CourseReviewDto(
            Id: review.Id,
            CourseId: review.CourseId,
            StudentId: review.StudentId,
            StudentName: review.Student?.FullName ?? string.Empty,
            StudentImageUrl: review.Student?.ImageUrl,
            Rating: review.Rating.Value,
            Comment: review.Comment,
            Status: review.Status.ToString(),
            CreatedAtUtc: review.CreatedAtUtc);
    }
}
