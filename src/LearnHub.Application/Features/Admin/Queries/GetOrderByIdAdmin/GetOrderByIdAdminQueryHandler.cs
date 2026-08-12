using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Admin.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Admin.Queries.GetOrderByIdAdmin;

public sealed class GetOrderByIdAdminQueryHandler(IAppDbContext context)
    : IRequestHandler<GetOrderByIdAdminQuery, Result<OrderAdminDetailDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<OrderAdminDetailDto>> Handle(
        GetOrderByIdAdminQuery request,
        CancellationToken cancellationToken)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .Include(o => o.Payments)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);

        if (order is null)
        {
            return Error.NotFound("Order.NotFound", "Order not found.");
        }

        var student = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == order.StudentId, cancellationToken);

        var courseIds = order.Items.Select(i => i.CourseId).Distinct().ToList();
        var courseTitles = await _context.Courses
            .AsNoTracking()
            .Where(c => courseIds.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id, c => c.Title, cancellationToken);

        var items = order.Items.Select(i => new OrderItemAdminDto(
            i.CourseId,
            courseTitles.TryGetValue(i.CourseId, out var title) ? title : string.Empty,
            i.UnitPriceSnapshot.Amount)).ToList();

        var payments = order.Payments.Select(p => new PaymentAdminSummaryDto(
            p.Id,
            p.OrderId,
            p.Provider.ToString(),
            p.Status.ToString(),
            p.Amount.Amount,
            p.Amount.Currency,
            p.TransactionId,
            p.ProviderReference,
            p.FailureReason,
            p.CreatedAtUtc)).ToList();

        return new OrderAdminDetailDto(
            order.Id,
            order.StudentId,
            student?.FullName ?? string.Empty,
            student?.Email ?? string.Empty,
            order.SubtotalAmount.Amount,
            order.DiscountAmount.Amount,
            order.TotalAmount.Amount,
            order.TotalAmount.Currency,
            order.Status.ToString(),
            order.AppliedCoupon?.Code,
            items,
            payments,
            order.CreatedAtUtc);
    }
}
