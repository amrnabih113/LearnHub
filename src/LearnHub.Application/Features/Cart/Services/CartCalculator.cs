using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Cart.Dtos;
using LearnHub.Domain.Courses;
using LearnHub.Domain.Purchasing.Carts;
using LearnHub.Domain.Purchasing.Coupons;
using LearnHub.Domain.Purchasing.Enums;
using LearnHub.Domain.Subscriptions;
using Microsoft.EntityFrameworkCore;

using DomainCart = LearnHub.Domain.Purchasing.Carts.Cart;

namespace LearnHub.Application.Features.Cart.Services;

public static class CartCalculator
{
    public static async Task<CartDto> CalculateAsync(
        DomainCart cart,
        IAppDbContext context,
        CancellationToken cancellationToken = default)
    {
        var courseIds = cart.Items.Select(i => i.CourseId).ToList();

        var courses = await context.Courses
            .AsNoTracking()
            .Where(c => courseIds.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id, cancellationToken);

        var nowUtc = DateTimeOffset.UtcNow;
        var activeSub = await context.Subscriptions
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.StudentId == cart.StudentId
                                   && (s.Status == SubscriptionStatus.Active || s.Status == SubscriptionStatus.Trialing)
                                   && s.ExpiresAtUtc > nowUtc, cancellationToken);

        SubscriptionPlan? plan = null;
        if (activeSub is not null && activeSub.SubscriptionPlanId != Guid.Empty)
        {
            plan = await context.SubscriptionPlans
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == activeSub.SubscriptionPlanId, cancellationToken);
        }

        var activeTier = plan?.Tier ?? activeSub?.Tier ?? SubscriptionTier.Free;

        var itemDtos = new List<CartItemDto>();
        decimal originalSubtotal = 0m;
        decimal payableSubtotal = 0m;

        foreach (var item in cart.Items)
        {
            courses.TryGetValue(item.CourseId, out var course);

            var originalPrice = item.UnitPrice.Amount;
            var isFree = originalPrice == 0m;

            var isCoveredBySubscription = false;
            if (!isFree && course is not null && course.IsIncludedInSubscription && activeSub is not null)
            {
                isCoveredBySubscription = activeTier >= course.RequiredSubscriptionTier;
            }

            var payableUnitPrice = (isFree || isCoveredBySubscription) ? 0m : originalPrice;

            originalSubtotal += originalPrice;
            payableSubtotal += payableUnitPrice;

            itemDtos.Add(new CartItemDto(
                CourseId: item.CourseId,
                CourseTitle: item.CourseTitle,
                OriginalUnitPrice: originalPrice,
                IsFree: isFree,
                IsCoveredBySubscription: isCoveredBySubscription,
                PayableUnitPrice: payableUnitPrice,
                Currency: item.UnitPrice.Currency));
        }

        decimal subscriptionDiscount = originalSubtotal - payableSubtotal;
        decimal couponDiscount = 0m;
        string? validCouponCode = null;

        if (!string.IsNullOrWhiteSpace(cart.CouponCode))
        {
            var code = cart.CouponCode.Trim().ToUpperInvariant();
            var coupon = await context.Coupons
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Code == code, cancellationToken);

            if (coupon is not null && IsCouponValid(coupon, cart.Currency, DateTimeOffset.UtcNow))
            {
                validCouponCode = coupon.Code;
                couponDiscount = CalculateCouponDiscount(coupon, itemDtos);
            }
        }

        decimal totalPayableAmount = Math.Max(0m, payableSubtotal - couponDiscount);

        return new CartDto(
            CartId: cart.Id,
            StudentId: cart.StudentId,
            Currency: cart.Currency,
            Items: itemDtos,
            OriginalSubtotal: originalSubtotal,
            SubscriptionDiscount: subscriptionDiscount,
            PayableSubtotal: payableSubtotal,
            CouponCode: validCouponCode,
            CouponDiscount: couponDiscount,
            TotalPayableAmount: totalPayableAmount);
    }

    public static bool IsCouponValid(Coupon coupon, string cartCurrency, DateTimeOffset nowUtc)
    {
        if (!coupon.IsActive) return false;
        if (coupon.ExpiresAtUtc.HasValue && nowUtc >= coupon.ExpiresAtUtc.Value) return false;
        if (coupon.MaxRedemptions.HasValue && coupon.RedemptionCount >= coupon.MaxRedemptions.Value) return false;
        if (!string.Equals(coupon.Currency, cartCurrency, StringComparison.OrdinalIgnoreCase)) return false;
        return true;
    }

    public static decimal CalculateCouponDiscount(Coupon coupon, IReadOnlyList<CartItemDto> items)
    {
        IEnumerable<CartItemDto> eligibleItems = items;
        if (coupon.AllowedCourseIds.Count > 0)
        {
            eligibleItems = items.Where(i => coupon.AllowedCourseIds.Contains(i.CourseId));
        }

        var discountableAmount = eligibleItems.Sum(i => i.PayableUnitPrice);
        if (discountableAmount <= 0m)
        {
            return 0m;
        }

        var discount = coupon.DiscountType switch
        {
            DiscountType.Percentage => decimal.Round(discountableAmount * coupon.DiscountValue / 100m, 2),
            DiscountType.FixedAmount => Math.Min(discountableAmount, coupon.DiscountValue),
            _ => 0m
        };

        return Math.Min(discount, discountableAmount);
    }
}
