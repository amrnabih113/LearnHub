using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Reviews.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Reviews.Queries.GetStudentInstructorReview;

public sealed class GetStudentInstructorReviewQueryHandler(IAppDbContext context)
    : IRequestHandler<GetStudentInstructorReviewQuery, Result<InstructorReviewDto?>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<InstructorReviewDto?>> Handle(
        GetStudentInstructorReviewQuery request,
        CancellationToken cancellationToken)
    {
        var review = await _context.InstructorReviews
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.InstructorId == request.InstructorId && r.StudentId == request.StudentId, cancellationToken);

        if (review is null)
        {
            return (InstructorReviewDto?)null;
        }

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
