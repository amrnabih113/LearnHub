using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Enrollments.Commands.CancelEnrollment;

public sealed class CancelEnrollmentCommandHandler(IAppDbContext context)
    : IRequestHandler<CancelEnrollmentCommand, Result<Updated>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<Updated>> Handle(CancelEnrollmentCommand request, CancellationToken cancellationToken)
    {
        var enrollment = await _context.Enrollments
            .FirstOrDefaultAsync(e => e.Id == request.EnrollmentId, cancellationToken);

        if (enrollment is null)
        {
            return Error.NotFound(
                code: "ApplicationError.Enrollment.NotFound",
                description: "Enrollment not found.");
        }

        var cancelResult = enrollment.Cancel();
        if (cancelResult.IsError)
        {
            return cancelResult.Errors;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Updated;
    }
}
