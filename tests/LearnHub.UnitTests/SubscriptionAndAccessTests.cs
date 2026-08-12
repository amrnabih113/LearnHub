using FluentAssertions;
using LearnHub.Domain.Common.ValueObjects;
using LearnHub.Domain.Classification.Categories;
using LearnHub.Domain.Courses;
using LearnHub.Domain.Courses.Enums;
using LearnHub.Domain.Purchasing.ValueObjects;
using LearnHub.Domain.Subscriptions;
using Xunit;

namespace LearnHub.UnitTests;

public class SubscriptionAndAccessTests
{
    [Fact]
    public void CanAccessCourse_FreeCourse_ShouldReturnTrue()
    {
        var course = CreateTestCourse(0m, isIncludedInSub: false, requiredTier: SubscriptionTier.Free);

        var canAccess = AccessPolicy.CanAccessCourse(course, subscription: null, purchased: false, trialOffer: null, now: DateTimeOffset.UtcNow);

        canAccess.Should().BeTrue();
    }

    [Fact]
    public void CanAccessCourse_ProSubAndProCourse_ShouldReturnTrue()
    {
        var course = CreateTestCourse(100m, isIncludedInSub: true, requiredTier: SubscriptionTier.Pro);
        var subscription = Subscription.Create(Guid.NewGuid(), Guid.NewGuid(), SubscriptionTier.Pro, BillingCycle.Monthly, DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddDays(30)).Value;
        subscription.Activate(DateTimeOffset.UtcNow);

        var canAccess = AccessPolicy.CanAccessCourse(course, subscription, purchased: false, trialOffer: null, now: DateTimeOffset.UtcNow);

        canAccess.Should().BeTrue();
    }

    [Fact]
    public void CanAccessCourse_FreeSubAndProCourse_ShouldReturnFalse()
    {
        var course = CreateTestCourse(100m, isIncludedInSub: true, requiredTier: SubscriptionTier.Pro);
        var subscription = Subscription.Create(Guid.NewGuid(), Guid.NewGuid(), SubscriptionTier.Free, BillingCycle.Monthly, DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddDays(30)).Value;
        subscription.Activate(DateTimeOffset.UtcNow);

        var canAccess = AccessPolicy.CanAccessCourse(course, subscription, purchased: false, trialOffer: null, now: DateTimeOffset.UtcNow);

        canAccess.Should().BeFalse();
    }

    private static Course CreateTestCourse(decimal priceAmount, bool isIncludedInSub, SubscriptionTier requiredTier)
    {
        return Course.Create(
            id: Guid.NewGuid(),
            title: "Test Course",
            description: "Test Description",
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
