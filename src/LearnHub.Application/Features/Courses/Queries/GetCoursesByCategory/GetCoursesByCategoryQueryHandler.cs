using LearnHub.Application.common.Models;
using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Courses.Dtos;
using LearnHub.Application.Features.Courses.Mappers;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Courses.Queries.GetCoursesByCategory;

public sealed class GetCoursesByCategoryQueryHandler(IAppDbContext context) : IRequestHandler<GetCoursesByCategoryQuery, Result<PagedResult<CourseDto>>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<PagedResult<CourseDto>>> Handle(GetCoursesByCategoryQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Courses
            .AsNoTracking()
            .Where(x => x.CategoryId == request.CategoryId);

        var totalCount = await query.CountAsync(cancellationToken);

        var courses = await query
            .OrderByDescending(x => x.CreatedAtUtc)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var totalPages = totalCount == 0
            ? 0
            : (int)Math.Ceiling(totalCount / (double)request.PageSize);

        return new PagedResult<CourseDto>
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
            Items = courses.Select(x => x.ToDto()).ToArray()
        };
    }
}