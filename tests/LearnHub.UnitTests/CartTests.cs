using FluentAssertions;
using LearnHub.Domain.Purchasing;
using LearnHub.Domain.Purchasing.Carts;
using LearnHub.Domain.Purchasing.ValueObjects;
using Xunit;

namespace LearnHub.UnitTests;

public class CartTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldReturnCart()
    {
        var studentId = Guid.NewGuid();
        var result = Cart.Create(Guid.NewGuid(), studentId, "USD");

        result.IsSuccess.Should().BeTrue();
        result.Value.StudentId.Should().Be(studentId);
        result.Value.Currency.Should().Be("USD");
        result.Value.Items.Should().BeEmpty();
    }

    [Fact]
    public void AddItem_WhenCourseAlreadyInCart_ShouldReturnError()
    {
        var cart = Cart.Create(Guid.NewGuid(), Guid.NewGuid(), "USD").Value;
        var courseId = Guid.NewGuid();
        var price = Money.Create(50m, "USD").Value;

        var addFirst = cart.AddItem(courseId, "Course A", price);
        addFirst.IsSuccess.Should().BeTrue();

        var addSecond = cart.AddItem(courseId, "Course A", price);
        addSecond.IsError.Should().BeTrue();
        addSecond.Errors[0].Code.Should().Be(CartErrors.ItemAlreadyAdded.Code);
    }

    [Fact]
    public void RemoveItem_WhenItemExists_ShouldRemoveItem()
    {
        var cart = Cart.Create(Guid.NewGuid(), Guid.NewGuid(), "USD").Value;
        var courseId = Guid.NewGuid();
        var price = Money.Create(50m, "USD").Value;

        cart.AddItem(courseId, "Course A", price);
        cart.Items.Should().HaveCount(1);

        var removeResult = cart.RemoveItem(courseId);
        removeResult.IsSuccess.Should().BeTrue();
        cart.Items.Should().BeEmpty();
    }

    [Fact]
    public void Clear_ShouldRemoveAllItemsAndCoupon()
    {
        var cart = Cart.Create(Guid.NewGuid(), Guid.NewGuid(), "USD").Value;
        cart.AddItem(Guid.NewGuid(), "Course A", Money.Create(50m, "USD").Value);
        cart.ApplyCoupon("DISCOUNT10");

        cart.Items.Should().HaveCount(1);
        cart.CouponCode.Should().Be("DISCOUNT10");

        cart.Clear();

        cart.Items.Should().BeEmpty();
        cart.CouponCode.Should().BeNull();
    }
}
