using LearnHub.Application.common.Interfaces;
using LearnHub.Application.common.Models;
using LearnHub.Application.Features.Reviews.Dtos;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Reviews.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Reviews.Queries.GetInstructorReviews;

public sealed class GetInstructorReviewsQueryHandler(IAppDbContext context)
    : IRequestHandler<GetInstructorReviewsQuery, Result<PagedResult<InstructorReviewDto>>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<PagedResult<InstructorReviewDto>>> Handle(
        GetInstructorReviewsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.InstructorReviews
            .AsNoTracking()
            .Where(r => r.InstructorId == request.InstructorId && r.Status == ReviewStatus.Published);

        var totalCount = await query.CountAsync(cancellationToken);

        var pageNumber = Math.Max(1, request.PageNumber);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var itemsRaw = await query
            .OrderByDescending(r => r.CreatedAtUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var studentIds = itemsRaw.Select(r => r.StudentId).Distinct().ToList();
        var students = await _context.Users
            .AsNoTracking()
            .Where(u => studentIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, cancellationToken);

        var items = itemsRaw.Select(r =>
        {
            students.TryGetValue(r.StudentId, out var student);
            return new InstructorReviewDto(
                r.Id,
                r.InstructorId,
                r.StudentId,
                student?.FullName ?? string.Empty,
                student?.ImageUrl,
                r.CourseId,
                r.Rating.Value,
                r.Comment,
                r.Status.ToString(),
                r.CreatedAtUtc);
        }).ToList();

        return new PagedResult<InstructorReviewDto>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}
