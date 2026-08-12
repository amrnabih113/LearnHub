using LearnHub.Application.Common.Interfaces;
using LearnHub.Application.Common.Interfaces.Authentication;

using LearnHub.Contracts.Payments.Requests;
using LearnHub.Application.Features.Payments.Commands.CreateCourseCheckout;
using LearnHub.Application.Features.Payments.Commands.CreateSubscriptionCheckout;
using LearnHub.Application.Features.Payments.Commands.ProcessPaymentWebhook;
using LearnHub.Application.Features.Payments.Queries.GetPaymentById;
using LearnHub.Application.Features.Payments.Queries.GetUserPayments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearnHub.Api.Controllers;

[Route("api/v1/payments")]
public sealed class PaymentsController(
    ISender sender,
    ICurrentUserService currentUserService) : BaseController
{
    private readonly ISender _sender = sender;
    private readonly ICurrentUserService _currentUserService = currentUserService;

    [HttpPost("course/checkout")]
    [Authorize]
    public async Task<IActionResult> CourseCheckout(
        [FromBody] CreateCourseCheckoutRequest request,
        CancellationToken cancellationToken)
    {
        var studentId = _currentUserService.UserId ?? Guid.Empty;
        var command = new CreateCourseCheckoutCommand(
            studentId,
            request.CourseId,
            request.SuccessUrl,
            request.CancelUrl);


        var result = await _sender.Send(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpPost("subscription/checkout")]
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

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetPaymentById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetPaymentByIdQuery(id);
        var result = await _sender.Send(query, cancellationToken);

        return HandleResult(result);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetMyPayments(CancellationToken cancellationToken)
    {
        var studentId = _currentUserService.UserId ?? Guid.Empty;
        var query = new GetUserPaymentsQuery(studentId);

        var result = await _sender.Send(query, cancellationToken);

        return HandleResult(result);
    }
}
