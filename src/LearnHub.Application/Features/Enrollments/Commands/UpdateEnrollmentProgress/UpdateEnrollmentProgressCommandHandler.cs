using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Enrollments.Commands.UpdateEnrollmentProgress;

public sealed class UpdateEnrollmentProgressCommandHandler(IAppDbContext context)
    : IRequestHandler<UpdateEnrollmentProgressCommand, Result<Updated>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<Updated>> Handle(UpdateEnrollmentProgressCommand request, CancellationToken cancellationToken)
    {
        var enrollment = await _context.Enrollments
            .Include(e => e.LessonsProgress)
            .FirstOrDefaultAsync(e => e.Id == request.EnrollmentId, cancellationToken);

        if (enrollment is null)
        {
            return Error.NotFound(
                code: "ApplicationError.Enrollment.NotFound",
                description: "Enrollment not found.");
        }

        var updateResult = enrollment.UpdateWatchProgress(
            request.LessonId,
            request.WatchDurationSeconds,
            request.TotalLessons,
            request.LessonDurationSeconds);

        if (updateResult.IsError)
        {
            return updateResult.Errors;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Updated;
    }
}
