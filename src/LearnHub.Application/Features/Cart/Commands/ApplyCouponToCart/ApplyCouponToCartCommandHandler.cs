using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Cart.Dtos;
using LearnHub.Application.Features.Cart.Services;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Purchasing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Cart.Commands.ApplyCouponToCart;

public sealed class ApplyCouponToCartCommandHandler(IAppDbContext context)
    : IRequestHandler<ApplyCouponToCartCommand, Result<CartDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<CartDto>> Handle(ApplyCouponToCartCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.CouponCode))
        {
            return CartErrors.CouponCodeRequired;
        }

        var normalizedCode = request.CouponCode.Trim().ToUpperInvariant();
        var coupon = await _context.Coupons
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Code == normalizedCode, cancellationToken);

        if (coupon is null)
        {
            return CartErrors.CouponNotFound;
        }

        if (!coupon.IsActive)
        {
            return CouponErrors.Inactive;
        }

        if (coupon.ExpiresAtUtc.HasValue && DateTimeOffset.UtcNow >= coupon.ExpiresAtUtc.Value)
        {
            return CouponErrors.Expired;
        }

        if (coupon.MaxRedemptions.HasValue && coupon.RedemptionCount >= coupon.MaxRedemptions.Value)
        {
            return CouponErrors.RedemptionLimitReached;
        }

        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.StudentId == request.StudentId, cancellationToken);

        if (cart is null || cart.Items.Count == 0)
        {
            return CartErrors.EmptyCart;
        }

        if (!string.Equals(coupon.Currency, cart.Currency, StringComparison.OrdinalIgnoreCase))
        {
            return CartErrors.InvalidCurrency;
        }

        var applyResult = cart.ApplyCoupon(normalizedCode);
        if (applyResult.IsError)
        {
            return applyResult.Errors;
        }

        await _context.SaveChangesAsync(cancellationToken);

        var calculatedCart = await CartCalculator.CalculateAsync(cart, _context, cancellationToken);
        if (calculatedCart.CouponDiscount <= 0m)
        {
            return CartErrors.CouponNotApplicable;
        }

        return calculatedCart;
    }
}
