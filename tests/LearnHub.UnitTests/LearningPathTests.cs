using FluentAssertions;
using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.LearningPaths.Commands.AddCourseToLearningPath;
using LearnHub.Application.Features.LearningPaths.Commands.CreateLearningPath;
using LearnHub.Application.Features.LearningPaths.Commands.PublishLearningPath;
using LearnHub.Application.Features.LearningPaths.Queries.GetLearningPathProgress;
using LearnHub.Domain.Classification.Categories;
using LearnHub.Domain.Courses;
using LearnHub.Domain.Courses.Enums;
using LearnHub.Domain.Enrollments;
using LearnHub.Domain.LearningPaths;
using LearnHub.Domain.LearningPaths.Enums;
using LearnHub.Domain.Purchasing.ValueObjects;
using LearnHub.Domain.Subscriptions;
using LearnHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LearnHub.UnitTests;

public class LearningPathTests
{
    private static AppDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public void CreateLearningPath_WithValidParameters_ShouldSucceedInDraftStatus()
    {
        // Act
        var result = LearningPath.Create(
            Guid.NewGuid(),
            "Full Stack .NET Developer",
            "full-stack-dotnet",
            "Become a senior .NET developer",
            "C#, ASP.NET Core, EF Core",
            "https://img.com/thumb.jpg",
            CourseLevel.Intermediate,
            Guid.NewGuid());

        // Assert
        result.IsSuccess.Should().BeTrue();
        var path = result.Value;
        path.Title.Should().Be("Full Stack .NET Developer");
        path.Status.Should().Be(LearningPathStatus.Draft);
        path.Courses.Should().BeEmpty();
    }

    [Fact]
    public void PublishLearningPath_WithoutCourses_ShouldFail()
    {
        // Arrange
        var path = LearningPath.Create(
            Guid.NewGuid(), "Empty Path", "empty", "Desc", "Short", null, CourseLevel.Beginner, null).Value;

        // Act
        var publishResult = path.Publish();

        // Assert
        publishResult.IsError.Should().BeTrue();
        publishResult.TopError.Code.Should().Be("LearningPath.CourseRequired");
    }

    [Fact]
    public void AddCourseAndReorder_ShouldMaintainOrderIndices()
    {
        // Arrange
        var path = LearningPath.Create(
            Guid.NewGuid(), "Backend Path", "backend", "Desc", "Short", null, CourseLevel.Beginner, null).Value;

        var c1 = Guid.NewGuid();
        var c2 = Guid.NewGuid();
        var c3 = Guid.NewGuid();

        // Act
        path.AddCourse(c1);
        path.AddCourse(c2);
        path.AddCourse(c3);

        // Reorder: c3, c1, c2
        var reorderResult = path.ReorderCourses([c3, c1, c2]);

        // Assert
        reorderResult.IsSuccess.Should().BeTrue();
        path.Courses.ElementAt(0).CourseId.Should().Be(c3);
        path.Courses.ElementAt(0).Order.Should().Be(1);
        path.Courses.ElementAt(1).CourseId.Should().Be(c1);
        path.Courses.ElementAt(1).Order.Should().Be(2);
        path.Courses.ElementAt(2).CourseId.Should().Be(c2);
        path.Courses.ElementAt(2).Order.Should().Be(3);
    }

    [Fact]
    public void AddDuplicateCourse_ShouldFail()
    {
        // Arrange
        var path = LearningPath.Create(
            Guid.NewGuid(), "Path", "path", "Desc", "Short", null, CourseLevel.Beginner, null).Value;
        var c1 = Guid.NewGuid();
        path.AddCourse(c1);

        // Act
        var duplicateResult = path.AddCourse(c1);

        // Assert
        duplicateResult.IsError.Should().BeTrue();
        duplicateResult.TopError.Code.Should().Be("LearningPath.CourseAlreadyInPath");
    }

    [Fact]
    public async Task ProgressCalculation_ShouldComputePercentageAndCurrentNextCourseFromEnrollments()
    {
        // Arrange
        using var dbContext = CreateInMemoryDbContext();
        var studentId = Guid.NewGuid();
        var category = Category.Create(Guid.NewGuid(), "Dev", "dev", "Desc").Value;
        dbContext.Categories.Add(category);

        var course1 = Course.Create(Guid.NewGuid(), "C# Basics", "Desc", Guid.NewGuid(), category.Id, null, CourseLevel.Beginner, CourseStatus.Published, Money.Create(0, "USD").Value, true, SubscriptionTier.Free, "en", "English", "US").Value;
        var course2 = Course.Create(Guid.NewGuid(), "ASP.NET Core", "Desc", Guid.NewGuid(), category.Id, null, CourseLevel.Intermediate, CourseStatus.Published, Money.Create(0, "USD").Value, true, SubscriptionTier.Free, "en", "English", "US").Value;
        var course3 = Course.Create(Guid.NewGuid(), "Clean Architecture", "Desc", Guid.NewGuid(), category.Id, null, CourseLevel.Advanced, CourseStatus.Published, Money.Create(0, "USD").Value, true, SubscriptionTier.Free, "en", "English", "US").Value;

        dbContext.Courses.AddRange(course1, course2, course3);

        var path = LearningPath.Create(Guid.NewGuid(), ".NET Path", "dotnet-path", "Desc", "Short", null, CourseLevel.Intermediate, null).Value;
        path.AddCourse(course1.Id);
        path.AddCourse(course2.Id);
        path.AddCourse(course3.Id);
        path.Publish();
        dbContext.LearningPaths.Add(path);

        // Student completed course 1
        var enrollment1 = Enrollment.Create(Guid.NewGuid(), studentId, course1.Id).Value;
        enrollment1.MarkCompleted();
        dbContext.Enrollments.Add(enrollment1);

        await dbContext.SaveChangesAsync();

        var handler = new GetLearningPathProgressQueryHandler(dbContext);

        // Act
        var result = await handler.Handle(new GetLearningPathProgressQuery(path.Id, studentId), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var progress = result.Value;
        progress.TotalCourses.Should().Be(3);
        progress.CompletedCourses.Should().Be(1);
        progress.ProgressPercentage.Should().Be(33.33m);
        progress.CurrentCourseId.Should().Be(course2.Id);
        progress.NextCourseId.Should().Be(course3.Id);
        progress.IsCompleted.Should().BeFalse();
    }
}
