using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Enrollments.Commands.CompleteEnrollment;

public sealed class CompleteEnrollmentCommandHandler(IAppDbContext context)
    : IRequestHandler<CompleteEnrollmentCommand, Result<Updated>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<Updated>> Handle(CompleteEnrollmentCommand request, CancellationToken cancellationToken)
    {
        var enrollment = await _context.Enrollments
            .Include(e => e.Certificate)
            .FirstOrDefaultAsync(e => e.Id == request.EnrollmentId, cancellationToken);

        if (enrollment is null)
        {
            return Error.NotFound(
                code: "ApplicationError.Enrollment.NotFound",
                description: "Enrollment not found.");
        }

        var markResult = enrollment.MarkCompleted();
        if (markResult.IsError)
        {
            return markResult.Errors;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Updated;
    }
}
