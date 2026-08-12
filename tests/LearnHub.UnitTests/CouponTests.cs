using FluentAssertions;
using LearnHub.Domain.Purchasing;
using LearnHub.Domain.Purchasing.Coupons;
using LearnHub.Domain.Purchasing.Enums;
using Xunit;

namespace LearnHub.UnitTests;

public class CouponTests
{
    [Fact]
    public void Create_ValidPercentageCoupon_ShouldCreateCoupon()
    {
        var result = Coupon.Create(Guid.NewGuid(), "SAVE20", DiscountType.Percentage, 20m, "USD");

        result.IsSuccess.Should().BeTrue();
        result.Value.Code.Should().Be("SAVE20");
        result.Value.DiscountType.Should().Be(DiscountType.Percentage);
        result.Value.DiscountValue.Should().Be(20m);
    }

    [Fact]
    public void Create_WhenExpiredDatePassed_ShouldReturnExpiredError()
    {
        var expiredDate = DateTimeOffset.UtcNow.AddDays(-1);
        var result = Coupon.Create(Guid.NewGuid(), "EXPIRED", DiscountType.FixedAmount, 10m, "USD", expiresAtUtc: expiredDate);

        result.IsError.Should().BeTrue();
        result.Errors[0].Code.Should().Be(CouponErrors.Expired.Code);
    }

    [Fact]
    public void Redeem_WhenMaxRedemptionsReached_ShouldReturnError()
    {
        var coupon = Coupon.Create(Guid.NewGuid(), "LIMITED", DiscountType.FixedAmount, 10m, "USD", maxRedemptions: 1).Value;

        var firstRedeem = coupon.Redeem(Guid.NewGuid());
        firstRedeem.IsSuccess.Should().BeTrue();

        var secondRedeem = coupon.Redeem(Guid.NewGuid());
        secondRedeem.IsError.Should().BeTrue();
        secondRedeem.Errors[0].Code.Should().Be(CouponErrors.RedemptionLimitReached.Code);
    }

    [Fact]
    public void ToSnapshot_ShouldReturnAccurateSnapshot()
    {
        var coupon = Coupon.Create(Guid.NewGuid(), "SNAPSHOT10", DiscountType.Percentage, 10m, "USD").Value;
        var snapshot = coupon.ToSnapshot();

        snapshot.Code.Should().Be("SNAPSHOT10");
        snapshot.DiscountType.Should().Be(DiscountType.Percentage);
        snapshot.DiscountValue.Should().Be(10m);
        snapshot.Currency.Should().Be("USD");
    }
}
