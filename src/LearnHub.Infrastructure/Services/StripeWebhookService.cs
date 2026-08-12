using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Common.Enums;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Purchasing.Enums;
using LearnHub.Domain.Subscriptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace LearnHub.Infrastructure.Services;

public sealed class StripeWebhookService(
    IOptions<StripeSettings> options,
    IAppDbContext context,
    ICourseAccessService courseAccessService) : IStripeWebhookService
{
    private readonly StripeSettings _settings = options.Value;
    private readonly IAppDbContext _context = context;
    private readonly ICourseAccessService _courseAccessService = courseAccessService;

    public async Task<Result<Updated>> ProcessWebhookAsync(
        string jsonPayload,
        string? signatureHeader,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(signatureHeader))
        {
            return Error.Validation("Stripe.MissingSignature", "Stripe-Signature header is missing.");
        }

        Event stripeEvent;
        try
        {
            stripeEvent = EventUtility.ConstructEvent(
                jsonPayload,
                signatureHeader,
                _settings.WebhookSecret,
                throwOnApiVersionMismatch: false);
        }
        catch (StripeException ex)
        {
            return Error.Validation("Stripe.WebhookError", ex.Message);
        }

        switch (stripeEvent.Type)
        {
            case EventTypes.CheckoutSessionCompleted:
                if (stripeEvent.Data.Object is Session session)
                {
                    await HandleCheckoutSessionCompletedAsync(session, cancellationToken);
                }
                break;

            case EventTypes.PaymentIntentSucceeded:
                if (stripeEvent.Data.Object is PaymentIntent paymentIntent)
                {
                    await HandlePaymentIntentSucceededAsync(paymentIntent, cancellationToken);
                }
                break;

            case EventTypes.PaymentIntentPaymentFailed:
                if (stripeEvent.Data.Object is PaymentIntent failedIntent)
                {
                    await HandlePaymentIntentFailedAsync(failedIntent, cancellationToken);
                }
                break;

            case EventTypes.CustomerSubscriptionCreated:
            case EventTypes.CustomerSubscriptionUpdated:
                if (stripeEvent.Data.Object is Stripe.Subscription subscription)
                {
                    await HandleSubscriptionUpdatedAsync(subscription, cancellationToken);
                }
                break;

            case EventTypes.CustomerSubscriptionDeleted:
                if (stripeEvent.Data.Object is Stripe.Subscription deletedSub)
                {
                    await HandleSubscriptionDeletedAsync(deletedSub, cancellationToken);
                }
                break;

            case EventTypes.InvoicePaid:
                if (stripeEvent.Data.Object is Invoice invoice)
                {
                    await HandleInvoicePaidAsync(invoice, cancellationToken);
                }
                break;

            case EventTypes.InvoicePaymentFailed:
                if (stripeEvent.Data.Object is Invoice failedInvoice)
                {
                    await HandleInvoicePaymentFailedAsync(failedInvoice, cancellationToken);
                }
                break;
        }

        return Result.Updated;
    }

    private async Task HandleCheckoutSessionCompletedAsync(Session session, CancellationToken cancellationToken)
    {
        if (session.Metadata is not null && session.Metadata.TryGetValue("orderId", out var orderIdStr) && Guid.TryParse(orderIdStr, out var orderId))
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);
            if (order is not null && order.Status != OrderStatus.Paid)
            {
                order.MarkPaid(session.PaymentIntentId ?? session.Id, DateTimeOffset.UtcNow);

                var payment = await _context.Payments.FirstOrDefaultAsync(p => p.OrderId == orderId, cancellationToken);
                if (payment is not null && payment.Status != PaymentStatus.Succeeded)
                {
                    payment.MarkSucceeded(session.PaymentIntentId ?? session.Id, session.Id, DateTimeOffset.UtcNow);
                }

                await _context.SaveChangesAsync(cancellationToken);
                await _courseAccessService.ProcessOrderPaymentSucceededAsync(orderId, cancellationToken);
            }
        }
        else if (session.Metadata is not null && session.Metadata.TryGetValue("subscriptionId", out var subIdStr) && Guid.TryParse(subIdStr, out var subId))
        {
            await ActivateSubscriptionAndSyncEnrollmentsAsync(subId, cancellationToken);
        }
    }

    private async Task HandlePaymentIntentSucceededAsync(PaymentIntent paymentIntent, CancellationToken cancellationToken)
    {
        if (paymentIntent.Metadata is not null && paymentIntent.Metadata.TryGetValue("orderId", out var orderIdStr) && Guid.TryParse(orderIdStr, out var orderId))
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);
            if (order is not null && order.Status != OrderStatus.Paid)
            {
                order.MarkPaid(paymentIntent.Id, DateTimeOffset.UtcNow);

                var payment = await _context.Payments.FirstOrDefaultAsync(p => p.OrderId == orderId, cancellationToken);
                if (payment is not null && payment.Status != PaymentStatus.Succeeded)
                {
                    payment.MarkSucceeded(paymentIntent.Id, paymentIntent.Id, DateTimeOffset.UtcNow);
                }

                await _context.SaveChangesAsync(cancellationToken);
                await _courseAccessService.ProcessOrderPaymentSucceededAsync(orderId, cancellationToken);
            }
        }
    }

    private async Task HandlePaymentIntentFailedAsync(PaymentIntent paymentIntent, CancellationToken cancellationToken)
    {
        if (paymentIntent.Metadata is not null && paymentIntent.Metadata.TryGetValue("orderId", out var orderIdStr) && Guid.TryParse(orderIdStr, out var orderId))
        {
            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.OrderId == orderId, cancellationToken);
            if (payment is not null && payment.Status != PaymentStatus.Failed)
            {
                payment.MarkFailed(paymentIntent.LastPaymentError?.Message ?? "Payment failed", DateTimeOffset.UtcNow);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }

    private async Task HandleSubscriptionUpdatedAsync(Stripe.Subscription subscription, CancellationToken cancellationToken)
    {
        if (subscription.Metadata is not null && subscription.Metadata.TryGetValue("studentId", out var studentIdStr) && Guid.TryParse(studentIdStr, out var studentId))
        {
            await _courseAccessService.SynchronizeUserEnrollmentsAsync(studentId, cancellationToken);
        }
    }

    private async Task HandleSubscriptionDeletedAsync(Stripe.Subscription subscription, CancellationToken cancellationToken)
    {
        if (subscription.Metadata is not null && subscription.Metadata.TryGetValue("studentId", out var studentIdStr) && Guid.TryParse(studentIdStr, out var studentId))
        {
            var activeSub = await _context.Subscriptions.FirstOrDefaultAsync(s => s.StudentId == studentId && s.Status == SubscriptionStatus.Active, cancellationToken);
            if (activeSub is not null)
            {
                activeSub.Cancel(DateTimeOffset.UtcNow);
                await _context.SaveChangesAsync(cancellationToken);
            }

            await _courseAccessService.SynchronizeUserEnrollmentsAsync(studentId, cancellationToken);
        }
    }

    private async Task HandleInvoicePaidAsync(Invoice invoice, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(invoice.Id))
        {
            var subPayment = await _context.SubscriptionPayments
                .FirstOrDefaultAsync(sp => sp.GatewayTransactionId == invoice.Id, cancellationToken);

            if (subPayment is not null && subPayment.Status != PaymentStatus.Succeeded)
            {
                subPayment.MarkSucceeded(invoice.Id, DateTimeOffset.UtcNow);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }

    private async Task HandleInvoicePaymentFailedAsync(Invoice invoice, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(invoice.Id))
        {
            var subPayment = await _context.SubscriptionPayments
                .FirstOrDefaultAsync(sp => sp.GatewayTransactionId == invoice.Id, cancellationToken);

            if (subPayment is not null && subPayment.Status != PaymentStatus.Failed)
            {
                subPayment.MarkFailed(DateTimeOffset.UtcNow, "Invoice payment failed");
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }

    private async Task ActivateSubscriptionAndSyncEnrollmentsAsync(Guid subscriptionId, CancellationToken cancellationToken)
    {
        var subscription = await _context.Subscriptions.FirstOrDefaultAsync(s => s.Id == subscriptionId, cancellationToken);
        if (subscription is not null)
        {
            subscription.Activate(DateTimeOffset.UtcNow);

            var subPayment = await _context.SubscriptionPayments.FirstOrDefaultAsync(sp => sp.SubscriptionId == subscriptionId, cancellationToken);
            if (subPayment is not null && subPayment.Status != PaymentStatus.Succeeded)
            {
                subPayment.MarkSucceeded($"SUB-{subscription.Id:N}", DateTimeOffset.UtcNow);
            }

            await _context.SaveChangesAsync(cancellationToken);
            await _courseAccessService.SynchronizeUserEnrollmentsAsync(subscription.StudentId, cancellationToken);
        }
    }
}
