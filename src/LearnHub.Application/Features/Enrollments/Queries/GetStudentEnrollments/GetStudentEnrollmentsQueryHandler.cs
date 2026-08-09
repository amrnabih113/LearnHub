using LearnHub.Application.common.Interfaces;
using LearnHub.Application.common.Models;
using LearnHub.Application.Features.Enrollments.Dtos;
using LearnHub.Application.Features.Enrollments.Mappers;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Enrollments.Queries.GetStudentEnrollments;

public sealed class GetStudentEnrollmentsQueryHandler(IAppDbContext context)
    : IRequestHandler<GetStudentEnrollmentsQuery, Result<PagedResult<EnrollmentDto>>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<PagedResult<EnrollmentDto>>> Handle(GetStudentEnrollmentsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Enrollments
            .AsNoTracking()
            .Include(e => e.Student)
            .Include(e => e.Course)
            .Where(e => e.StudentId == request.StudentId);

        var totalCount = await query.CountAsync(cancellationToken);

        var enrollments = await query
            .OrderByDescending(e => e.CreatedAtUtc)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var totalPages = totalCount == 0
            ? 0
            : (int)Math.Ceiling(totalCount / (double)request.PageSize);

        return new PagedResult<EnrollmentDto>
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
            Items = enrollments.Select(e => e.ToDto()).ToArray()
        };
    }
}
