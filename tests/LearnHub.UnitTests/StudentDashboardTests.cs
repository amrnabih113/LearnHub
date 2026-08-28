using FluentAssertions;
using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Student.Commands.UpdateStudentProfile;
using LearnHub.Application.Features.Student.Queries.GetStudentLearningDashboard;
using LearnHub.Application.Features.Student.Queries.GetStudentProfile;
using LearnHub.Application.Features.Student.Queries.GetStudentStatistics;
using LearnHub.Domain.Classification.Categories;
using LearnHub.Domain.Courses;
using LearnHub.Domain.Courses.Enums;
using LearnHub.Domain.Courses.Sections;
using LearnHub.Domain.Courses.Sections.Lessons;
using LearnHub.Domain.Enrollments;
using LearnHub.Domain.Enrollments.LessonProgress;
using LearnHub.Domain.Identity;
using LearnHub.Domain.Purchasing.ValueObjects;
using LearnHub.Domain.Subscriptions;
using LearnHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace LearnHub.UnitTests;

public class StudentDashboardTests
{
    private static AppDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task GetStudentProfile_ShouldReturnStudentDetails()
    {
        // Arrange
        using var dbContext = CreateInMemoryDbContext();
        var userResult = User.Create(
            Guid.NewGuid(), "John", "Doe", "john@example.com", "Hash123", Role.Student);
        var user = userResult.Value;
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        var handler = new GetStudentProfileQueryHandler(dbContext);

        // Act
        var result = await handler.Handle(new GetStudentProfileQuery(user.Id), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Email.Should().Be("john@example.com");
        result.Value.FullName.Should().Be("John Doe");
    }

    [Fact]
    public async Task UpdateStudentProfile_ShouldMutateProfileViaDomain()
    {
        // Arrange
        using var dbContext = CreateInMemoryDbContext();
        var userResult = User.Create(
            Guid.NewGuid(), "Jane", "Doe", "jane@example.com", "Hash123", Role.Student);
        var user = userResult.Value;
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        var handler = new UpdateStudentProfileCommandHandler(dbContext);
        var command = new UpdateStudentProfileCommand(
            user.Id, "Janet", "Smith", "+1234567890", new DateOnly(1995, 5, 20), "Bio text", "US");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.FirstName.Should().Be("Janet");
        result.Value.LastName.Should().Be("Smith");
        result.Value.FullName.Should().Be("Janet Smith");
    }

    [Fact]
    public async Task GetStudentStatistics_ShouldCalculateLearningTimeAndStreak()
    {
        // Arrange
        using var dbContext = CreateInMemoryDbContext();
        var studentId = Guid.NewGuid();
        var courseId = Guid.NewGuid();

        var enrollment = Enrollment.Create(Guid.NewGuid(), studentId, courseId).Value;
        dbContext.Enrollments.Add(enrollment);

        var lesson1 = LessonProgress.Create(Guid.NewGuid(), enrollment.Id, Guid.NewGuid()).Value;
        lesson1.UpdateWatchProgress(1800); // 30 minutes
        dbContext.LessonProgresses.Add(lesson1);

        await dbContext.SaveChangesAsync();

        var handler = new GetStudentStatisticsQueryHandler(dbContext);

        // Act
        var result = await handler.Handle(new GetStudentStatisticsQuery(studentId), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.EnrolledCourses.Should().Be(1);
        result.Value.LearningTimeThisWeekMinutes.Should().Be(30);
        result.Value.CurrentStreakDays.Should().BeGreaterThanOrEqualTo(1);
    }
}
