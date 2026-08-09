using System.Text.Json;
using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Purchasing.Commands.ProcessStripeWebhook;

public sealed class ProcessStripeWebhookCommandHandler(ICourseAccessService courseAccessService)
    : IRequestHandler<ProcessStripeWebhookCommand, Result<Updated>>
{
    private readonly ICourseAccessService _courseAccessService = courseAccessService;

    public async Task<Result<Updated>> Handle(ProcessStripeWebhookCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.JsonPayload))
        {
            return Error.Validation("StripeWebhook.EmptyPayload", "Webhook payload is empty.");
        }

        try
        {
            using var doc = JsonDocument.Parse(request.JsonPayload);
            var root = doc.RootElement;

            if (!root.TryGetProperty("type", out var typeElement))
            {
                return Result.Updated;
            }

            var eventType = typeElement.GetString();

            if (eventType is "checkout.session.completed" or "payment_intent.succeeded")
            {
                if (root.TryGetProperty("data", out var data) &&
                    data.TryGetProperty("object", out var obj) &&
                    obj.TryGetProperty("metadata", out var metadata) &&
                    metadata.TryGetProperty("orderId", out var orderIdElement) &&
                    Guid.TryParse(orderIdElement.GetString(), out var orderId))
                {
                    await _courseAccessService.ProcessOrderPaymentSucceededAsync(orderId, cancellationToken);
                }
            }
            else if (eventType is "customer.subscription.created" or "customer.subscription.updated" or "customer.subscription.deleted")
            {
                if (root.TryGetProperty("data", out var data) &&
                    data.TryGetProperty("object", out var obj) &&
                    obj.TryGetProperty("metadata", out var metadata) &&
                    metadata.TryGetProperty("studentId", out var studentIdElement) &&
                    Guid.TryParse(studentIdElement.GetString(), out var studentId))
                {
                    await _courseAccessService.SynchronizeUserEnrollmentsAsync(studentId, cancellationToken);
                }
            }

            return Result.Updated;
        }
        catch (JsonException)
        {
            return Error.Validation("StripeWebhook.InvalidJson", "Invalid JSON payload format.");
        }
    }
}
