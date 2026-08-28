using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Student.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Student.Queries.GetStudentOrders;

public sealed record GetStudentOrdersQuery(Guid StudentId)
    : IRequest<Result<IReadOnlyList<StudentOrderDto>>>;

public sealed class GetStudentOrdersQueryHandler(IAppDbContext context)
    : IRequestHandler<GetStudentOrdersQuery, Result<IReadOnlyList<StudentOrderDto>>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<IReadOnlyList<StudentOrderDto>>> Handle(
        GetStudentOrdersQuery request,
        CancellationToken cancellationToken)
    {
        var orders = await _context.Orders
            .Include(o => o.Items)
            .AsNoTracking()
            .Where(o => o.StudentId == request.StudentId)
            .OrderByDescending(o => o.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        var courseIds = orders.SelectMany(o => o.Items.Select(i => i.CourseId)).Distinct().ToList();
        var courseTitles = await _context.Courses
            .AsNoTracking()
            .Where(c => courseIds.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id, c => c.Title, cancellationToken);

        var dtos = orders.Select(o =>
        {
            var titles = o.Items
                .Select(i => courseTitles.TryGetValue(i.CourseId, out var t) ? t : "Course")
                .ToList();

            return new StudentOrderDto(
                o.Id,
                o.CreatedAtUtc,
                o.Status.ToString(),
                o.TotalAmount.Amount,
                o.TotalAmount.Currency,
                o.Items.Count,
                titles);
        }).ToList();

        return dtos;
    }
}
