using FluentAssertions;
using LearnHub.Application.Features.Instructor.Queries.GetInstructorDashboard;
using LearnHub.Domain.Classification.Categories;
using LearnHub.Domain.Courses;
using LearnHub.Domain.Courses.Enums;
using LearnHub.Domain.Identity;
using LearnHub.Domain.Purchasing.Orders;
using LearnHub.Domain.Purchasing.ValueObjects;
using LearnHub.Domain.Subscriptions;
using LearnHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LearnHub.UnitTests;

public class InstructorDashboardTests
{
    private static AppDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task GetInstructorDashboard_ShouldAggregateInstructorCoursesAndRevenue()
    {
        // Arrange
        using var dbContext = CreateInMemoryDbContext();
        var instructorId = Guid.NewGuid();
        var category = Category.Create(Guid.NewGuid(), "Web Dev", "web-dev", "Desc").Value;
        dbContext.Categories.Add(category);

        var course = Course.Create(
            Guid.NewGuid(), "Mastering ASP.NET Core", "Description",
            instructorId, category.Id, null, CourseLevel.Intermediate, CourseStatus.Published,
            Money.Create(100, "USD").Value, false, SubscriptionTier.Free, "en", "English", "US").Value;
        dbContext.Courses.Add(course);

        // Paid order containing instructor course
        var studentId = Guid.NewGuid();
        var order = Order.Create(Guid.NewGuid(), studentId, "USD").Value;
        order.AddItem(course.Id, course.Title, Money.Create(100, "USD").Value);
        order.Checkout(DateTimeOffset.UtcNow);
        order.MarkPaid("PAY_123", DateTimeOffset.UtcNow);
        dbContext.Orders.Add(order);

        await dbContext.SaveChangesAsync();

        var handler = new GetInstructorDashboardQueryHandler(dbContext);

        // Act
        var result = await handler.Handle(new GetInstructorDashboardQuery(instructorId), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.TotalCourses.Should().Be(1);
        result.Value.PublishedCourses.Should().Be(1);
        result.Value.TotalRevenue.Should().Be(100m);
    }

    [Fact]
    public async Task GetInstructorDashboard_ShouldIsolateOtherInstructorsData()
    {
        // Arrange
        using var dbContext = CreateInMemoryDbContext();
        var instructor1 = Guid.NewGuid();
        var instructor2 = Guid.NewGuid();
        var category = Category.Create(Guid.NewGuid(), "Mobile", "mobile", "Desc").Value;
        dbContext.Categories.Add(category);

        var course1 = Course.Create(
            Guid.NewGuid(), "Flutter Masterclass", "Desc",
            instructor1, category.Id, null, CourseLevel.Beginner, CourseStatus.Published,
            Money.Create(50, "USD").Value, false, SubscriptionTier.Free, "en", "English", "US").Value;

        var course2 = Course.Create(
            Guid.NewGuid(), "iOS Swift UI", "Desc",
            instructor2, category.Id, null, CourseLevel.Advanced, CourseStatus.Published,
            Money.Create(200, "USD").Value, false, SubscriptionTier.Free, "en", "English", "US").Value;

        dbContext.Courses.AddRange(course1, course2);
        await dbContext.SaveChangesAsync();

        var handler = new GetInstructorDashboardQueryHandler(dbContext);

        // Act
        var result = await handler.Handle(new GetInstructorDashboardQuery(instructor1), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.TotalCourses.Should().Be(1);
        result.Value.TopCourses.Should().OnlyContain(c => c.Title == "Flutter Masterclass");
    }
}
