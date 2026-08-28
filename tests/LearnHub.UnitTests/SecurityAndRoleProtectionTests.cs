using FluentAssertions;
using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Common.Interfaces.Authentication;
using LearnHub.Application.Features.Admin.Commands.AssignRole;

using LearnHub.Application.Features.Identity.Commands.RegisterInstructor;
using LearnHub.Application.Features.Identity.Commands.RegisterStudent;
using LearnHub.Application.Features.Instructor.Commands.AddInstructorLink;
using LearnHub.Domain.Identity;
using LearnHub.Domain.Instructor;
using LearnHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace LearnHub.UnitTests;

public sealed class SecurityAndRoleProtectionTests
{
    private static AppDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task StudentRegistration_AutomaticallyAssignsStudentRole_HardcodingRoleBoundary()
    {
        // Arrange
        using var dbContext = CreateInMemoryDbContext();
        var mockHasher = new Mock<IPasswordHasher>();
        mockHasher.Setup(h => h.HashPassword(It.IsAny<string>()))
            .Returns("hashed_secure_password");

        var handler = new RegisterStudentCommandHandler(dbContext, mockHasher.Object);
        var command = new RegisterStudentCommand(
            "StudentFirstName",
            "StudentLastName",
            "student@learnhub.com",
            "Password123!",
            "Password123!");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var user = await dbContext.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Email == "student@learnhub.com");
        user.Should().NotBeNull();
        user!.Roles.Should().HaveCount(1);
        user.Roles.First().Role.Should().Be(Role.Student);
        user.Roles.Should().NotContain(r => r.Role == Role.Admin || r.Role == Role.Instructor);
    }

    [Fact]
    public async Task InstructorRegistration_CreatesInstructorRoleAndPendingProfile()
    {
        // Arrange
        using var dbContext = CreateInMemoryDbContext();
        var mockHasher = new Mock<IPasswordHasher>();
        mockHasher.Setup(h => h.HashPassword(It.IsAny<string>()))
            .Returns("hashed_secure_password");

        var handler = new RegisterInstructorCommandHandler(dbContext, mockHasher.Object);
        var command = new RegisterInstructorCommand(
            "InstructorFirst",
            "InstructorLast",
            "instructor@learnhub.com",
            "Password123!",
            "Password123!",
            "Senior Backend Engineer",
            "ASP.NET Core Specialist",
            "Professional bio...");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var user = await dbContext.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Email == "instructor@learnhub.com");
        user.Should().NotBeNull();
        user!.Roles.Should().HaveCount(1);
        user.Roles.First().Role.Should().Be(Role.Instructor);

        var profile = await dbContext.InstructorProfiles.FirstOrDefaultAsync(p => p.UserId == user.Id);
        profile.Should().NotBeNull();
        profile!.VerificationStatus.Should().Be(InstructorVerificationStatus.Pending);
        profile.ProfessionalTitle.Should().Be("Senior Backend Engineer");
    }

    [Fact]
    public void InstructorLink_DangerousUrlSchemes_ShouldBeRejected()
    {
        // Act 1: Javascript scheme
        var jsResult = InstructorLink.Create(Guid.NewGuid(), Guid.NewGuid(), "XSS Link", "javascript:alert(1)");
        // Act 2: File scheme
        var fileResult = InstructorLink.Create(Guid.NewGuid(), Guid.NewGuid(), "Local File", "file:///C:/Windows/system32");
        // Act 3: Valid HTTPS scheme
        var httpsResult = InstructorLink.Create(Guid.NewGuid(), Guid.NewGuid(), "LinkedIn", "https://linkedin.com/in/instructor");

        // Assert
        jsResult.IsError.Should().BeTrue();
        fileResult.IsError.Should().BeTrue();
        httpsResult.IsSuccess.Should().BeTrue();
        httpsResult.Value.Url.Should().Be("https://linkedin.com/in/instructor");
    }

    [Fact]
    public void CalculateProfileCompletionPercentage_ShouldReflectStoredInformation()
    {
        // Arrange
        var profile = InstructorProfile.Create(
            Guid.NewGuid(),
            "Senior Software Engineer",
            "Headline",
            "Detailed bio...").Value;

        // Act 1: Initial creation (3 fields set out of 6)
        int initialCompletion = profile.CalculateCompletionPercentage();

        // Act 2: Add image, skill, and experience
        profile.UpdateProfileImage("https://cloud.com/image.png");
        profile.AddSkill(InstructorSkill.Create(Guid.NewGuid(), profile.Id, "C#").Value);
        profile.AddExperience(InstructorExperience.Create(
            Guid.NewGuid(), profile.Id, "Tech Lead", "Company", "Desc",
            new DateOnly(2020, 1, 1), null, true, "Remote").Value);

        int updatedCompletion = profile.CalculateCompletionPercentage();

        // Assert
        initialCompletion.Should().Be(50); // 3 / 6 * 100
        updatedCompletion.Should().Be(100); // 6 / 6 * 100
    }
}
