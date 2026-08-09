using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Payments.Commands.ProcessPaymentWebhook;

public sealed class ProcessPaymentWebhookCommandHandler(IStripeWebhookService webhookService)
    : IRequestHandler<ProcessPaymentWebhookCommand, Result<Updated>>
{
    private readonly IStripeWebhookService _webhookService = webhookService;

    public async Task<Result<Updated>> Handle(
        ProcessPaymentWebhookCommand request,
        CancellationToken cancellationToken)
    {
        return await _webhookService.ProcessWebhookAsync(
            request.JsonPayload,
            request.SignatureHeader,
            cancellationToken);
    }
}
