using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Payments.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Payments.Queries.GetPaymentById;

public sealed class GetPaymentByIdQueryHandler(IAppDbContext context)
    : IRequestHandler<GetPaymentByIdQuery, Result<PaymentDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<PaymentDto>> Handle(
        GetPaymentByIdQuery request,
        CancellationToken cancellationToken)
    {
        var payment = await _context.Payments
            .Include(p => p.Order)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.PaymentId, cancellationToken);

        if (payment is not null)
        {
            return new PaymentDto(
                PaymentId: payment.Id,
                UserId: payment.Order?.StudentId ?? Guid.Empty,
                PaymentType: "CoursePurchase",
                StripeSessionId: payment.ProviderReference,
                StripePaymentIntentId: payment.TransactionId,
                StripeCustomerId: null,
                Amount: payment.Amount.Amount,
                Currency: payment.Amount.Currency,
                Status: payment.Status.ToString(),
                CreatedAt: payment.CreatedAtUtc,
                PaidAt: payment.SucceededAtUtc,
                FailureReason: payment.FailureReason);
        }

        var subPayment = await _context.SubscriptionPayments
            .Include(sp => sp.Subscription)
            .AsNoTracking()
            .FirstOrDefaultAsync(sp => sp.Id == request.PaymentId, cancellationToken);

        if (subPayment is not null)
        {
            return new PaymentDto(
                PaymentId: subPayment.Id,
                UserId: subPayment.Subscription?.StudentId ?? Guid.Empty,
                PaymentType: "SubscriptionPurchase",
                StripeSessionId: null,
                StripePaymentIntentId: subPayment.GatewayTransactionId,
                StripeCustomerId: null,
                Amount: subPayment.Amount.Amount,
                Currency: subPayment.Amount.Currency,
                Status: subPayment.Status.ToString(),
                CreatedAt: subPayment.CreatedAtUtc,
                PaidAt: subPayment.SucceededAtUtc,
                FailureReason: subPayment.FailureReason);
        }

        return Error.NotFound("Payment.NotFound", "Payment record not found.");
    }
}
