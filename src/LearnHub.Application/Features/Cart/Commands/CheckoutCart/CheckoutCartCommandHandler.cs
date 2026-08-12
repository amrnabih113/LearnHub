using LearnHub.Application.common.Errors;
using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Cart.Dtos;
using LearnHub.Application.Features.Cart.Services;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Enrollments.Enums;
using LearnHub.Domain.Purchasing;
using LearnHub.Domain.Purchasing.Enums;
using LearnHub.Domain.Purchasing.Orders;
using LearnHub.Domain.Purchasing.Payments;
using LearnHub.Domain.Purchasing.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Cart.Commands.CheckoutCart;

public sealed class CheckoutCartCommandHandler(
    IAppDbContext context,
    IPaymentGatewayService paymentGatewayService,
    ICourseAccessService courseAccessService)
    : IRequestHandler<CheckoutCartCommand, Result<CartCheckoutDto>>
{
    private readonly IAppDbContext _context = context;
    private readonly IPaymentGatewayService _paymentGatewayService = paymentGatewayService;
    private readonly ICourseAccessService _courseAccessService = courseAccessService;

    public async Task<Result<CartCheckoutDto>> Handle(
        CheckoutCartCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == request.StudentId, cancellationToken);

        if (user is null)
        {
            return ApplicationErrors.UserNotFound;
        }

        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.StudentId == request.StudentId, cancellationToken);

        if (cart is null || cart.Items.Count == 0)
        {
            return CartErrors.EmptyCart;
        }

        var calculatedCart = await CartCalculator.CalculateAsync(cart, _context, cancellationToken);

        foreach (var item in cart.Items)
        {
            var existingEnrollment = await _context.Enrollments
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.StudentId == user.Id && e.CourseId == item.CourseId, cancellationToken);

            if (existingEnrollment is not null && existingEnrollment.Status is EnrollmentStatus.Active or EnrollmentStatus.Completed)
            {
                return CartErrors.CourseAlreadyEnrolled;
            }
        }

        CouponSnapshot? couponSnapshot = null;
        LearnHub.Domain.Purchasing.Coupons.Coupon? coupon = null;

        if (!string.IsNullOrWhiteSpace(calculatedCart.CouponCode) && calculatedCart.CouponDiscount > 0m)
        {
            coupon = await _context.Coupons
                .FirstOrDefaultAsync(c => c.Code == calculatedCart.CouponCode, cancellationToken);

            if (coupon is not null && CartCalculator.IsCouponValid(coupon, cart.Currency, DateTimeOffset.UtcNow))
            {
                couponSnapshot = coupon.ToSnapshot();
            }
        }

        var orderResult = Order.Create(Guid.NewGuid(), user.Id, cart.Currency);
        if (orderResult.IsError)
        {
            return orderResult.Errors;
        }

        var order = orderResult.Value;

        foreach (var item in calculatedCart.Items)
        {
            var unitPriceResult = Money.Create(item.PayableUnitPrice, item.Currency);
            if (unitPriceResult.IsError)
            {
                return unitPriceResult.Errors;
            }

            var addItemResult = order.AddItem(item.CourseId, item.CourseTitle, unitPriceResult.Value);
            if (addItemResult.IsError)
            {
                return addItemResult.Errors;
            }
        }

        if (couponSnapshot is not null)
        {
            var applyCouponResult = order.ApplyCoupon(couponSnapshot);
            if (applyCouponResult.IsError)
            {
                return applyCouponResult.Errors;
            }
        }

        var now = DateTimeOffset.UtcNow;
        var checkoutResult = order.Checkout(now);
        if (checkoutResult.IsError)
        {
            return checkoutResult.Errors;
        }

        _context.Orders.Add(order);

        if (order.TotalAmount.Amount == 0m)
        {
            if (coupon is not null && cart.Items.Count > 0)
            {
                coupon.Redeem(cart.Items.First().CourseId);
            }

            await _context.SaveChangesAsync(cancellationToken);

            foreach (var item in order.Items)
            {
                await _courseAccessService.EnsureEnrollmentForCourseAccessAsync(user.Id, item.CourseId, cancellationToken);
            }

            cart.Clear();
            await _context.SaveChangesAsync(cancellationToken);

            return new CartCheckoutDto(
                OrderId: order.Id,
                PaymentId: null,
                SessionId: null,
                CheckoutUrl: request.SuccessUrl,
                Amount: 0m,
                Currency: order.Currency,
                RequiresPayment: false);
        }
        else
        {
            var paymentResult = Payment.Create(Guid.NewGuid(), order.Id, PaymentProvider.Stripe, order.TotalAmount);
            if (paymentResult.IsError)
            {
                return paymentResult.Errors;
            }

            var payment = paymentResult.Value;
            _context.Payments.Add(payment);

            if (coupon is not null && cart.Items.Count > 0)
            {
                coupon.Redeem(cart.Items.First().CourseId);
            }

            await _context.SaveChangesAsync(cancellationToken);

            var itemTitles = string.Join(", ", calculatedCart.Items.Select(i => i.CourseTitle));

            var args = new CreateCheckoutSessionArgs(
                UserId: user.Id,
                UserEmail: user.Email,
                PaymentType: PaymentType.CoursePurchase,
                TargetId: order.Id,
                ItemTitle: itemTitles,
                Amount: order.TotalAmount.Amount,
                Currency: order.TotalAmount.Currency,
                SuccessUrl: request.SuccessUrl,
                CancelUrl: request.CancelUrl,
                Metadata: new Dictionary<string, string>
                {
                    ["orderId"] = order.Id.ToString(),
                    ["paymentId"] = payment.Id.ToString(),
                    ["studentId"] = user.Id.ToString()
                });

            var sessionResult = await _paymentGatewayService.CreateCheckoutSessionAsync(args, cancellationToken);
            if (sessionResult.IsError)
            {
                return sessionResult.Errors;
            }

            return new CartCheckoutDto(
                OrderId: order.Id,
                PaymentId: payment.Id,
                SessionId: sessionResult.Value.SessionId,
                CheckoutUrl: sessionResult.Value.CheckoutUrl,
                Amount: order.TotalAmount.Amount,
                Currency: order.Currency,
                RequiresPayment: true);
        }
    }
}
