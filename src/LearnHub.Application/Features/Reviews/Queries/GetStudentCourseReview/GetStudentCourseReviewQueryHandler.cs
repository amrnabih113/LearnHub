using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Reviews.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Reviews.Queries.GetStudentCourseReview;

public sealed class GetStudentCourseReviewQueryHandler(IAppDbContext context)
    : IRequestHandler<GetStudentCourseReviewQuery, Result<CourseReviewDto?>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<CourseReviewDto?>> Handle(
        GetStudentCourseReviewQuery request,
        CancellationToken cancellationToken)
    {
        var review = await _context.CourseReviews
            .Include(r => r.Student)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.CourseId == request.CourseId && r.StudentId == request.StudentId, cancellationToken);

        if (review is null)
        {
            return (CourseReviewDto?)null;
        }

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
