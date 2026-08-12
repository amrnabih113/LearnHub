using LearnHub.Application.common.Errors;
using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Reviews.Dtos;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Enrollments.Enums;
using LearnHub.Domain.Reviews;
using LearnHub.Domain.Reviews.InstructorReviews;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Reviews.Commands.CreateInstructorReview;

public sealed class CreateInstructorReviewCommandHandler(IAppDbContext context)
    : IRequestHandler<CreateInstructorReviewCommand, Result<InstructorReviewDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<InstructorReviewDto>> Handle(
        CreateInstructorReviewCommand request,
        CancellationToken cancellationToken)
    {
        var student = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == request.StudentId, cancellationToken);
        if (student is null)
        {
            return ApplicationErrors.UserNotFound;
        }

        var instructorExists = await _context.Users
            .AsNoTracking()
            .AnyAsync(u => u.Id == request.InstructorId, cancellationToken);
        if (!instructorExists)
        {
            return Error.NotFound("Instructor.NotFound", "Instructor not found.");
        }

        var instructorCourseIds = await _context.Courses
            .AsNoTracking()
            .Where(c => c.InstructorId == request.InstructorId)
            .Select(c => c.Id)
            .ToListAsync(cancellationToken);

        var hasEnrollment = await _context.Enrollments
            .AsNoTracking()
            .AnyAsync(e => e.StudentId == request.StudentId
                        && instructorCourseIds.Contains(e.CourseId)
                        && (e.Status == EnrollmentStatus.Active || e.Status == EnrollmentStatus.Completed), cancellationToken);

        if (!hasEnrollment)
        {
            return ReviewErrors.NotEnrolledWithInstructor;
        }

        var existingReview = await _context.InstructorReviews
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.InstructorId == request.InstructorId
                                   && r.StudentId == request.StudentId
                                   && r.CourseId == request.CourseId, cancellationToken);

        if (existingReview is not null)
        {
            return ReviewErrors.DuplicateReview;
        }

        var reviewResult = InstructorReview.Create(Guid.NewGuid(), request.InstructorId, request.StudentId, request.CourseId, request.Rating, request.Comment);
        if (reviewResult.IsError)
        {
            return reviewResult.Errors;
        }

        var review = reviewResult.Value;
        review.Publish();

        _context.InstructorReviews.Add(review);
        await _context.SaveChangesAsync(cancellationToken);

        return new InstructorReviewDto(
            Id: review.Id,
            InstructorId: review.InstructorId,
            StudentId: review.StudentId,
            StudentName: student.FullName,
            StudentImageUrl: student.ImageUrl,
            CourseId: review.CourseId,
            Rating: review.Rating.Value,
            Comment: review.Comment,
            Status: review.Status.ToString(),
            CreatedAtUtc: review.CreatedAtUtc);
    }
}
