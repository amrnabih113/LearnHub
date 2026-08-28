using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Student.Dtos;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Purchasing.Orders;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Student.Queries.GetStudentOrderById;

public sealed record GetStudentOrderByIdQuery(Guid StudentId, Guid OrderId)
    : IRequest<Result<StudentOrderDetailDto>>;

public sealed class GetStudentOrderByIdQueryHandler(IAppDbContext context)
    : IRequestHandler<GetStudentOrderByIdQuery, Result<StudentOrderDetailDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<StudentOrderDetailDto>> Handle(
        GetStudentOrderByIdQuery request,
        CancellationToken cancellationToken)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == request.OrderId && o.StudentId == request.StudentId, cancellationToken);

        if (order is null)
        {
            return Error.NotFound("Order.NotFound", "Order was not found.");
        }

        var courseIds = order.Items.Select(i => i.CourseId).Distinct().ToList();
        var courseTitles = await _context.Courses
            .AsNoTracking()
            .Where(c => courseIds.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id, c => c.Title, cancellationToken);

        var itemDtos = order.Items.Select(i => new StudentOrderItemDto(
            CourseId: i.CourseId,
            CourseTitle: courseTitles.TryGetValue(i.CourseId, out var t) ? t : i.CourseTitle,
            Price: i.UnitPriceSnapshot.Amount,
            Currency: i.UnitPriceSnapshot.Currency
        )).ToList();

        return new StudentOrderDetailDto(
            OrderId: order.Id,
            OrderDate: order.CreatedAtUtc,
            Status: order.Status.ToString(),
            TotalAmount: order.TotalAmount.Amount,
            Currency: order.TotalAmount.Currency,
            Items: itemDtos);
    }
}
