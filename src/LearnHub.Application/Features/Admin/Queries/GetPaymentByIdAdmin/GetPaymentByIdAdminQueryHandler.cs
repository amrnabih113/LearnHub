using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Admin.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Admin.Queries.GetPaymentByIdAdmin;

public sealed class GetPaymentByIdAdminQueryHandler(IAppDbContext context)
    : IRequestHandler<GetPaymentByIdAdminQuery, Result<PaymentAdminSummaryDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<PaymentAdminSummaryDto>> Handle(
        GetPaymentByIdAdminQuery request,
        CancellationToken cancellationToken)
    {
        var payment = await _context.Payments
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (payment is null)
        {
            return Error.NotFound("Payment.NotFound", "Payment not found.");
        }

        return new PaymentAdminSummaryDto(
            payment.Id,
            payment.OrderId,
            payment.Provider.ToString(),
            payment.Status.ToString(),
            payment.Amount.Amount,
            payment.Amount.Currency,
            payment.TransactionId,
            payment.ProviderReference,
            payment.FailureReason,
            payment.CreatedAtUtc);
    }
}
