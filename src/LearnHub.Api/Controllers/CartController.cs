using LearnHub.Application.Common.Interfaces.Authentication;
using LearnHub.Application.Features.Cart.Commands.AddToCart;
using LearnHub.Application.Features.Cart.Commands.ApplyCouponToCart;
using LearnHub.Application.Features.Cart.Commands.CheckoutCart;
using LearnHub.Application.Features.Cart.Commands.ClearCart;
using LearnHub.Application.Features.Cart.Commands.RemoveCouponFromCart;
using LearnHub.Application.Features.Cart.Commands.RemoveFromCart;
using LearnHub.Application.Features.Cart.Queries.GetCart;
using LearnHub.Contracts.Cart.Requests;
using LearnHub.Contracts.Cart.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearnHub.Api.Controllers;

[Route("api/v1/cart")]
[Authorize]
public sealed class CartController(
    ISender sender,
    ICurrentUserService currentUserService) : BaseController
{
    private readonly ISender _sender = sender;
    private readonly ICurrentUserService _currentUserService = currentUserService;

    [HttpGet]
    public async Task<IActionResult> GetCart(CancellationToken cancellationToken)
    {
        var studentId = _currentUserService.UserId ?? Guid.Empty;
        var query = new GetCartQuery(studentId);
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsError)
        {
            return HandleResult(result);
        }

        var response = MapToResponse(result.Value);
        return Ok(response);
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddToCart(
        [FromBody] AddToCartRequest request,
        CancellationToken cancellationToken)
    {
        var studentId = _currentUserService.UserId ?? Guid.Empty;
        var command = new AddToCartCommand(studentId, request.CourseId);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsError)
        {
            return HandleResult(result);
        }

        var response = MapToResponse(result.Value);
        return Ok(response);
    }

    [HttpDelete("items/{courseId:guid}")]
    public async Task<IActionResult> RemoveFromCart(
        Guid courseId,
        CancellationToken cancellationToken)
    {
        var studentId = _currentUserService.UserId ?? Guid.Empty;
        var command = new RemoveFromCartCommand(studentId, courseId);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsError)
        {
            return HandleResult(result);
        }

        var response = MapToResponse(result.Value);
        return Ok(response);
    }

    [HttpPost("coupon")]
    public async Task<IActionResult> ApplyCoupon(
        [FromBody] ApplyCouponRequest request,
        CancellationToken cancellationToken)
    {
        var studentId = _currentUserService.UserId ?? Guid.Empty;
        var command = new ApplyCouponToCartCommand(studentId, request.CouponCode);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsError)
        {
            return HandleResult(result);
        }

        var response = MapToResponse(result.Value);
        return Ok(response);
    }

    [HttpDelete("coupon")]
    public async Task<IActionResult> RemoveCoupon(CancellationToken cancellationToken)
    {
        var studentId = _currentUserService.UserId ?? Guid.Empty;
        var command = new RemoveCouponFromCartCommand(studentId);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsError)
        {
            return HandleResult(result);
        }

        var response = MapToResponse(result.Value);
        return Ok(response);
    }

    [HttpDelete]
    public async Task<IActionResult> ClearCart(CancellationToken cancellationToken)
    {
        var studentId = _currentUserService.UserId ?? Guid.Empty;
        var command = new ClearCartCommand(studentId);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsError)
        {
            return HandleResult(result);
        }

        var response = MapToResponse(result.Value);
        return Ok(response);
    }

    [HttpPost("checkout")]
    [HttpPost("/api/v1/checkout")]
    public async Task<IActionResult> Checkout(
        [FromBody] CheckoutCartRequest request,
        CancellationToken cancellationToken)
    {
        var studentId = _currentUserService.UserId ?? Guid.Empty;
        var command = new CheckoutCartCommand(studentId, request.SuccessUrl, request.CancelUrl);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsError)
        {
            return HandleResult(result);
        }

        var response = new CartCheckoutResponse(
            OrderId: result.Value.OrderId,
            PaymentId: result.Value.PaymentId,
            SessionId: result.Value.SessionId,
            CheckoutUrl: result.Value.CheckoutUrl,
            Amount: result.Value.Amount,
            Currency: result.Value.Currency,
            RequiresPayment: result.Value.RequiresPayment);

        return Ok(response);
    }

    private static CartResponse MapToResponse(Application.Features.Cart.Dtos.CartDto dto)
    {
        var items = dto.Items.Select(i => new CartItemResponse(
            CourseId: i.CourseId,
            CourseTitle: i.CourseTitle,
            OriginalUnitPrice: i.OriginalUnitPrice,
            IsFree: i.IsFree,
            IsCoveredBySubscription: i.IsCoveredBySubscription,
            PayableUnitPrice: i.PayableUnitPrice,
            Currency: i.Currency)).ToList();

        return new CartResponse(
            CartId: dto.CartId,
            StudentId: dto.StudentId,
            Currency: dto.Currency,
            Items: items,
            OriginalSubtotal: dto.OriginalSubtotal,
            SubscriptionDiscount: dto.SubscriptionDiscount,
            PayableSubtotal: dto.PayableSubtotal,
            CouponCode: dto.CouponCode,
            CouponDiscount: dto.CouponDiscount,
            TotalPayableAmount: dto.TotalPayableAmount);
    }
}
