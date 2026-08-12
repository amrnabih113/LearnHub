using FluentAssertions;
using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Cart.Commands.AddToCart;
using LearnHub.Application.Features.Cart.Commands.CheckoutCart;
using LearnHub.Application.Features.Cart.Services;
using LearnHub.Domain.Courses;
using LearnHub.Domain.Courses.Enums;
using LearnHub.Domain.Identity;
using LearnHub.Domain.Purchasing.Carts;
using LearnHub.Domain.Purchasing.Enums;
using LearnHub.Domain.Purchasing.ValueObjects;
using LearnHub.Domain.Subscriptions;
using LearnHub.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace LearnHub.UnitTests;

public class CheckoutAndCartIntegrationTests
{
    private readonly DbContextOptions<AppDbContext> _dbOptions;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IPaymentGatewayService> _paymentGatewayMock;
    private readonly Mock<ICourseAccessService> _courseAccessMock;

    public CheckoutAndCartIntegrationTests()
    {
        _dbOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _mediatorMock = new Mock<IMediator>();
        _paymentGatewayMock = new Mock<IPaymentGatewayService>();
        _courseAccessMock = new Mock<ICourseAccessService>();
    }

    private AppDbContext CreateDbContext() => new AppDbContext(_dbOptions, _mediatorMock.Object);

    [Fact]
    public async Task AddToCartHandler_WhenUserAndCourseValid_ShouldAddToCart()
    {
        using var context = CreateDbContext();
        var user = User.Create(Guid.NewGuid(), "Test", "User", "test@learnhub.com", "hash", Role.Student).Value;
        var course = CreateCourse("C# Masterclass", 100m);
        context.Users.Add(user);
        context.Courses.Add(course);
        await context.SaveChangesAsync();

        var handler = new AddToCartCommandHandler(context);
        var command = new AddToCartCommand(user.Id, course.Id);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().HaveCount(1);
        result.Value.Items.First().CourseId.Should().Be(course.Id);
        result.Value.TotalPayableAmount.Should().Be(100m);
    }

    [Fact]
    public async Task FreeCheckout_ShouldCreatePaidOrderAndNoStripePayment()
    {
        using var context = CreateDbContext();
        var user = User.Create(Guid.NewGuid(), "Free", "User", "free@learnhub.com", "hash", Role.Student).Value;
        var freeCourse = CreateCourse("Free C#", 0m);
        context.Users.Add(user);
        context.Courses.Add(freeCourse);

        var cart = Cart.Create(Guid.NewGuid(), user.Id, "USD").Value;
        cart.AddItem(freeCourse.Id, freeCourse.Title, freeCourse.Price);
        context.Carts.Add(cart);
        await context.SaveChangesAsync();

        _courseAccessMock.Setup(s => s.EnsureEnrollmentForCourseAccessAsync(user.Id, freeCourse.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.NewGuid());

        var handler = new CheckoutCartCommandHandler(context, _paymentGatewayMock.Object, _courseAccessMock.Object);
        var command = new CheckoutCartCommand(user.Id, "https://success", "https://cancel");

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.RequiresPayment.Should().BeFalse();
        result.Value.Amount.Should().Be(0m);

        var createdOrder = await context.Orders.FirstOrDefaultAsync(o => o.Id == result.Value.OrderId);
        createdOrder.Should().NotBeNull();
        createdOrder!.Status.Should().Be(OrderStatus.Paid);

        _paymentGatewayMock.Verify(p => p.CreateCheckoutSessionAsync(It.IsAny<CreateCheckoutSessionArgs>(), It.IsAny<CancellationToken>()), Times.Never);
        _courseAccessMock.Verify(c => c.EnsureEnrollmentForCourseAccessAsync(user.Id, freeCourse.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task MixedCart_ProSubscription_ShouldCalculatePayableCorrectly()
    {
        using var context = CreateDbContext();
        var user = User.Create(Guid.NewGuid(), "Pro", "User", "pro@learnhub.com", "hash", Role.Student).Value;

        var courseA = CreateCourse("Course A", 50m, isIncludedInSub: true, requiredTier: SubscriptionTier.Pro);
        var courseB = CreateCourse("Course B", 30m, isIncludedInSub: false, requiredTier: SubscriptionTier.Premium);
        var courseC = CreateCourse("Course C", 0m, isIncludedInSub: false, requiredTier: SubscriptionTier.Free);

        context.Users.Add(user);
        context.Courses.AddRange(courseA, courseB, courseC);

        var subPlanResult = SubscriptionPlan.Create(Guid.NewGuid(), "Pro Plan", SubscriptionTier.Pro, BillingCycle.Monthly, Money.Create(29m, "USD").Value);
        var subPlan = subPlanResult.Value;
        context.SubscriptionPlans.Add(subPlan);

        var subscription = Subscription.Create(Guid.NewGuid(), user.Id, SubscriptionTier.Pro, BillingCycle.Monthly, DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddDays(30)).Value;
        subscription.Activate(DateTimeOffset.UtcNow);
        context.Subscriptions.Add(subscription);

        var cart = Cart.Create(Guid.NewGuid(), user.Id, "USD").Value;
        cart.AddItem(courseA.Id, courseA.Title, courseA.Price);
        cart.AddItem(courseB.Id, courseB.Title, courseB.Price);
        cart.AddItem(courseC.Id, courseC.Title, courseC.Price);
        context.Carts.Add(cart);
        await context.SaveChangesAsync();

        var cartDto = await CartCalculator.CalculateAsync(cart, context);

        cartDto.OriginalSubtotal.Should().Be(80m);
        cartDto.SubscriptionDiscount.Should().Be(50m);
        cartDto.PayableSubtotal.Should().Be(30m);
        cartDto.TotalPayableAmount.Should().Be(30m);
    }

    private static Course CreateCourse(string title, decimal priceAmount, bool isIncludedInSub = true, SubscriptionTier requiredTier = SubscriptionTier.Pro)
    {
        return Course.Create(
            id: Guid.NewGuid(),
            title: title,
            description: "Description",
            instructorId: Guid.NewGuid(),
            categoryId: Guid.NewGuid(),
            thumbnailUrl: null,
            level: CourseLevel.Beginner,
            status: CourseStatus.Published,
            price: Money.Create(priceAmount, "USD").Value,
            isIncludedInSubscription: isIncludedInSub,
            requiredSubscriptionTier: requiredTier,
            language: "en",
            languageName: "English",
            country: null).Value;
    }
}
