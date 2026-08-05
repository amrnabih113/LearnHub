using LearnHub.Application.common.Models;
using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Courses.Dtos;
using LearnHub.Application.Features.Courses.Mappers;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Courses.Queries.GetCourses;

public sealed class GetCoursesQueryHandler(IAppDbContext context) : IRequestHandler<GetCoursesQuery, Result<PagedResult<CourseDto>>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<PagedResult<CourseDto>>> Handle(GetCoursesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Courses.AsNoTracking();

        if (request.CategoryId.HasValue)
        {
            query = query.Where(x => x.CategoryId == request.CategoryId.Value);
        }

        if (request.InstructorId.HasValue)
        {
            query = query.Where(x => x.InstructorId == request.InstructorId.Value);
        }

        if (request.Level.HasValue)
        {
            query = query.Where(x => x.Level == request.Level.Value);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(x => x.Status == request.Status.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Language))
        {
            var language = request.Language.Trim();
            query = query.Where(x => x.Language.Code == language || x.Language.Name.Contains(language));
        }

        if (request.MinPrice.HasValue)
        {
            query = query.Where(x => x.Price.Amount >= request.MinPrice.Value);
        }

        if (request.MaxPrice.HasValue)
        {
            query = query.Where(x => x.Price.Amount <= request.MaxPrice.Value);
        }

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