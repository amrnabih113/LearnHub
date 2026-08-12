using LearnHub.Application.Common.Interfaces.Authentication;
using LearnHub.Contracts.Payments.Requests;
using LearnHub.Contracts.Subscriptions.Requests;
using LearnHub.Application.Features.Payments.Commands.CreateSubscriptionCheckout;
using LearnHub.Application.Features.Payments.Commands.ProcessPaymentWebhook;
using LearnHub.Application.Features.Subscriptions.Commands.CancelSubscription;
using LearnHub.Application.Features.Subscriptions.Commands.ChangeSubscriptionPlan;
using LearnHub.Application.Features.Subscriptions.Commands.ResumeSubscription;
using LearnHub.Application.Features.Subscriptions.Queries.GetCurrentSubscription;
using LearnHub.Application.Features.Subscriptions.Queries.GetSubscriptionHistory;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearnHub.Api.Controllers;

[Route("api/v1/subscriptions")]
public sealed class SubscriptionsController(
    ISender sender,
    ICurrentUserService currentUserService) : BaseController
{
    private readonly ISender _sender = sender;
    private readonly ICurrentUserService _currentUserService = currentUserService;

    [HttpPost("checkout")]
    [Authorize]
    public async Task<IActionResult> SubscriptionCheckout(
        [FromBody] CreateSubscriptionCheckoutRequest request,
        CancellationToken cancellationToken)
    {
        var studentId = _currentUserService.UserId ?? Guid.Empty;
        var command = new CreateSubscriptionCheckoutCommand(
            studentId,
            request.SubscriptionPlanId,
            request.SuccessUrl,
            request.CancelUrl);

        var result = await _sender.Send(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpPost("webhook")]
    [AllowAnonymous]
    public async Task<IActionResult> HandleWebhook(CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(Request.Body);
        var jsonPayload = await reader.ReadToEndAsync(cancellationToken);
        var signatureHeader = Request.Headers["Stripe-Signature"].ToString();

        var command = new ProcessPaymentWebhookCommand(jsonPayload, signatureHeader);
        var result = await _sender.Send(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpGet("current")]
    [Authorize]
    public async Task<IActionResult> GetCurrentSubscription(CancellationToken cancellationToken)
    {
        var studentId = _currentUserService.UserId ?? Guid.Empty;
        var query = new GetCurrentSubscriptionQuery(studentId);
        var result = await _sender.Send(query, cancellationToken);

        return HandleResult(result);
    }

    [HttpGet("history")]
    [Authorize]
    public async Task<IActionResult> GetSubscriptionHistory(CancellationToken cancellationToken)
    {
        var studentId = _currentUserService.UserId ?? Guid.Empty;
        var query = new GetSubscriptionHistoryQuery(studentId);
        var result = await _sender.Send(query, cancellationToken);

        return HandleResult(result);
    }

    [HttpPost("cancel")]
    [Authorize]
    public async Task<IActionResult> CancelSubscription(CancellationToken cancellationToken)
    {
        var studentId = _currentUserService.UserId ?? Guid.Empty;
        var command = new CancelSubscriptionCommand(studentId);
        var result = await _sender.Send(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpPost("resume")]
    [Authorize]
    public async Task<IActionResult> ResumeSubscription(CancellationToken cancellationToken)
    {
        var studentId = _currentUserService.UserId ?? Guid.Empty;
        var command = new ResumeSubscriptionCommand(studentId);
        var result = await _sender.Send(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpPost("change-plan")]
    [Authorize]
    public async Task<IActionResult> ChangePlan(
        [FromBody] ChangeSubscriptionPlanRequest request,
        CancellationToken cancellationToken)
    {
        var studentId = _currentUserService.UserId ?? Guid.Empty;
        var command = new ChangeSubscriptionPlanCommand(studentId, request.NewPlanId);
        var result = await _sender.Send(command, cancellationToken);

        return HandleResult(result);
    }
}
