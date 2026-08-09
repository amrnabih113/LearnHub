using LearnHub.Application.Features.Purchasing.Commands.ProcessStripeWebhook;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LearnHub.Api.Controllers;

[Route("api/v1/webhooks")]
public sealed class WebhooksController(ISender sender) : BaseController
{
    private readonly ISender _sender = sender;

    [HttpPost("stripe")]
    public async Task<IActionResult> HandleStripeWebhook(CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(Request.Body);
        var jsonPayload = await reader.ReadToEndAsync(cancellationToken);
        var signatureHeader = Request.Headers["Stripe-Signature"].ToString();

        var command = new ProcessStripeWebhookCommand(jsonPayload, signatureHeader);
        var result = await _sender.Send(command, cancellationToken);

        return HandleResult(result);
    }
}
