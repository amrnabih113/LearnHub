using FluentAssertions;
using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Reviews.Commands.CreateCourseReview;
using LearnHub.Application.Features.Reviews.Commands.UpdateCourseReview;
using LearnHub.Application.Features.Reviews.Queries.GetCourseReviewSummary;
using LearnHub.Domain.Classification.Categories;
using LearnHub.Domain.Common.ValueObjects;
using LearnHub.Domain.Courses;
using LearnHub.Domain.Courses.Enums;
using LearnHub.Domain.Enrollments;
using LearnHub.Domain.Identity;
using LearnHub.Domain.Purchasing.ValueObjects;
using LearnHub.Domain.Reviews;
using LearnHub.Domain.Subscriptions;
using LearnHub.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace LearnHub.UnitTests;

public class ReviewsTests
{
    private readonly DbContextOptions<AppDbContext> _dbOptions;
    private readonly Mock<IMediator> _mediatorMock;

    public ReviewsTests()
    {
        _dbOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _mediatorMock = new Mock<IMediator>();
    }

    private AppDbContext CreateDbContext() => new AppDbContext(_dbOptions, _mediatorMock.Object);

    [Fact]
    public async Task CreateCourseReview_WhenStudentNotEnrolled_ShouldReturnError()
    {
        using var context = CreateDbContext();
        var student = User.Create(Guid.NewGuid(), "Alice", "Student", "alice@learnhub.com", "hash", Role.Student).Value;
        var course = CreateCourse("C# Advanced", 99m);
        context.Users.Add(student);
        context.Courses.Add(course);
        await context.SaveChangesAsync();

        var handler = new CreateCourseReviewCommandHandler(context);
        var command = new CreateCourseReviewCommand(course.Id, student.Id, 5, "Great course!");

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsError.Should().BeTrue();
        result.Errors[0].Code.Should().Be(ReviewErrors.NotEnrolledInCourse.Code);
    }

    [Fact]
    public async Task CreateCourseReview_WhenStudentEnrolled_ShouldCreateAndPublishReview()
    {
        using var context = CreateDbContext();
        var student = User.Create(Guid.NewGuid(), "Bob", "Student", "bob@learnhub.com", "hash", Role.Student).Value;
        var course = CreateCourse("C# Basics", 49m);
        var enrollment = Enrollment.Create(Guid.NewGuid(), student.Id, course.Id).Value;

        context.Users.Add(student);
        context.Courses.Add(course);
        context.Enrollments.Add(enrollment);
        await context.SaveChangesAsync();

        var handler = new CreateCourseReviewCommandHandler(context);
        var command = new CreateCourseReviewCommand(course.Id, student.Id, 5, "Awesome course!");

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Rating.Should().Be(5);
        result.Value.Comment.Should().Be("Awesome course!");
        result.Value.Status.Should().Be("Published");
    }

    [Fact]
    public async Task CreateCourseReview_WhenDuplicateReview_ShouldReturnError()
    {
        using var context = CreateDbContext();
        var student = User.Create(Guid.NewGuid(), "Charlie", "Student", "charlie@learnhub.com", "hash", Role.Student).Value;
        var course = CreateCourse("EF Core In Depth", 79m);
        var enrollment = Enrollment.Create(Guid.NewGuid(), student.Id, course.Id).Value;

        context.Users.Add(student);
        context.Courses.Add(course);
        context.Enrollments.Add(enrollment);
        await context.SaveChangesAsync();

        var handler = new CreateCourseReviewCommandHandler(context);
        var command = new CreateCourseReviewCommand(course.Id, student.Id, 4, "Good content");

        var firstResult = await handler.Handle(command, CancellationToken.None);
        firstResult.IsSuccess.Should().BeTrue();

        var secondResult = await handler.Handle(command, CancellationToken.None);
        secondResult.IsError.Should().BeTrue();
        secondResult.Errors[0].Code.Should().Be(ReviewErrors.DuplicateReview.Code);
    }

    [Fact]
    public async Task GetCourseReviewSummary_ShouldCalculateAverageAndDistributionCorrectly()
    {
        using var context = CreateDbContext();
        var student1 = User.Create(Guid.NewGuid(), "User1", "Test", "user1@test.com", "hash", Role.Student).Value;
        var student2 = User.Create(Guid.NewGuid(), "User2", "Test", "user2@test.com", "hash", Role.Student).Value;
        var course = CreateCourse("Clean Architecture", 120m);

        context.Users.AddRange(student1, student2);
        context.Courses.Add(course);
        context.Enrollments.Add(Enrollment.Create(Guid.NewGuid(), student1.Id, course.Id).Value);
        context.Enrollments.Add(Enrollment.Create(Guid.NewGuid(), student2.Id, course.Id).Value);
        await context.SaveChangesAsync();

        var handler = new CreateCourseReviewCommandHandler(context);
        await handler.Handle(new CreateCourseReviewCommand(course.Id, student1.Id, 5, "5 stars"), CancellationToken.None);
        await handler.Handle(new CreateCourseReviewCommand(course.Id, student2.Id, 3, "3 stars"), CancellationToken.None);

        var summaryHandler = new GetCourseReviewSummaryQueryHandler(context);
        var summaryResult = await summaryHandler.Handle(new GetCourseReviewSummaryQuery(course.Id), CancellationToken.None);

        summaryResult.IsSuccess.Should().BeTrue();
        summaryResult.Value.TotalReviews.Should().Be(2);
        summaryResult.Value.AverageRating.Should().Be(4.0);
        summaryResult.Value.StarCounts[5].Should().Be(1);
        summaryResult.Value.StarCounts[3].Should().Be(1);
    }

    private static Course CreateCourse(string title, decimal priceAmount)
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
            isIncludedInSubscription: true,
            requiredSubscriptionTier: SubscriptionTier.Pro,
            language: "en",
            languageName: "English",
            country: null).Value;
    }
}
