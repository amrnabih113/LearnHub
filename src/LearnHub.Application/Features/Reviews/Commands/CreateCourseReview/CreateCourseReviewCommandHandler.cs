using LearnHub.Application.common.Errors;
using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Reviews.Dtos;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Enrollments.Enums;
using LearnHub.Domain.Reviews;
using LearnHub.Domain.Reviews.CourseReviews;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Reviews.Commands.CreateCourseReview;

public sealed class CreateCourseReviewCommandHandler(IAppDbContext context)
    : IRequestHandler<CreateCourseReviewCommand, Result<CourseReviewDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<CourseReviewDto>> Handle(
        CreateCourseReviewCommand request,
        CancellationToken cancellationToken)
    {
        var student = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == request.StudentId, cancellationToken);
        if (student is null)
        {
            return ApplicationErrors.UserNotFound;
        }

        var course = await _context.Courses
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.CourseId, cancellationToken);
        if (course is null)
        {
            return Error.NotFound("Course.NotFound", "Course not found.");
        }

        var hasEnrollment = await _context.Enrollments
            .AsNoTracking()
            .AnyAsync(e => e.StudentId == request.StudentId
                        && e.CourseId == request.CourseId
                        && (e.Status == EnrollmentStatus.Active || e.Status == EnrollmentStatus.Completed), cancellationToken);

        if (!hasEnrollment)
        {
            return ReviewErrors.NotEnrolledInCourse;
        }

        var existingReview = await _context.CourseReviews
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.CourseId == request.CourseId && r.StudentId == request.StudentId, cancellationToken);

        if (existingReview is not null)
        {
            return ReviewErrors.DuplicateReview;
        }

        var reviewResult = CourseReview.Create(Guid.NewGuid(), request.CourseId, request.StudentId, request.Rating, request.Comment);
        if (reviewResult.IsError)
        {
            return reviewResult.Errors;
        }

        var review = reviewResult.Value;
        review.Publish();

        _context.CourseReviews.Add(review);
        await _context.SaveChangesAsync(cancellationToken);

        return new CourseReviewDto(
            Id: review.Id,
            CourseId: review.CourseId,
            StudentId: review.StudentId,
            StudentName: student.FullName,
            StudentImageUrl: student.ImageUrl,
            Rating: review.Rating.Value,
            Comment: review.Comment,
            Status: review.Status.ToString(),
            CreatedAtUtc: review.CreatedAtUtc);
    }
}
