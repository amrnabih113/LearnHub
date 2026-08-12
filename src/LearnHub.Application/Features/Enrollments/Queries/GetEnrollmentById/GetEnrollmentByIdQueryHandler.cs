using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Enrollments.Dtos;
using LearnHub.Application.Features.Enrollments.Mappers;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Enrollments.Queries.GetEnrollmentById;

public sealed class GetEnrollmentByIdQueryHandler(IAppDbContext context)
    : IRequestHandler<GetEnrollmentByIdQuery, Result<EnrollmentDetailsDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<EnrollmentDetailsDto>> Handle(GetEnrollmentByIdQuery request, CancellationToken cancellationToken)
    {
        var enrollment = await _context.Enrollments
            .AsNoTracking()
            .Include(e => e.Student)
            .Include(e => e.Course)
            .Include(e => e.Certificate)
            .Include(e => e.LessonsProgress)
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

        if (enrollment is null)
        {
            return Error.NotFound(
                code: "ApplicationError.Enrollment.NotFound",
                description: "Enrollment not found.");
        }

        return enrollment.ToDetailsDto();
    }
}
