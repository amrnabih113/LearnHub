using FluentAssertions;
using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Common.Interfaces.Authentication;
using LearnHub.Application.Features.Courses.Commands.CreateCourse;
using LearnHub.Application.Features.Courses.Commands.PublishCourse;
using LearnHub.Application.Features.Courses.Queries.GetCourseContent;
using LearnHub.Application.Features.Courses.Queries.GetCourseReadiness;
using LearnHub.Application.Features.Lessons.Commands.CreateLesson;
using LearnHub.Application.Features.Lessons.Commands.PublishLesson;
using LearnHub.Application.Features.Lessons.Commands.ReorderLessons;
using LearnHub.Application.Features.Lessons.Commands.UpdateLesson;
using LearnHub.Application.Features.Lessons.Commands.UploadLessonVideo;
using LearnHub.Application.Features.Sections.Commands.CreateSection;
using LearnHub.Application.Features.Sections.Commands.PublishSection;
using LearnHub.Application.Features.Sections.Commands.ReorderSections;
using LearnHub.Domain.Classification.Categories;
using LearnHub.Domain.Courses.Enums;
using LearnHub.Domain.Identity;
using LearnHub.Domain.Purchasing.ValueObjects;
using LearnHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace LearnHub.UnitTests;

public sealed class IncrementalCourseCreationTests
{
    private static AppDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task CreateDraftCourse_WithMinimalInfo_ShouldSucceedAndHaveDraftStatus()
    {
        // Arrange
        using var dbContext = CreateInMemoryDbContext();
        var mockStorage = new Mock<IFileStorageService>();

        var instructor = User.Create(
            Guid.NewGuid(),
            "Instructor",
            "User",
            "instructor@learnhub.com",
            "hashed_password",
            Role.Instructor).Value;

        var category = Category.Create(Guid.NewGuid(), "Programming", "Tech courses").Value;

        dbContext.Users.Add(instructor);
        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync();

        var handler = new CreateCourseCommandHandler(dbContext, mockStorage.Object);
        var command = new CreateCourseCommand(
            Title: "Minimal C# Course",
            Description: string.Empty,
            InstructorId: instructor.Id,
            CategoryId: category.Id,
            Thumbnail: null,
            Level: CourseLevel.Beginner,
            Status: CourseStatus.Draft,
            Price: Money.Create(0, "USD").Value,
            IsIncludedInSubscription: false,
            RequiredSubscriptionTier: Domain.Subscriptions.SubscriptionTier.Free,
            Language: "en",
            LanguageName: "English",
            Country: null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var course = await dbContext.Courses.FindAsync(result.Value);
        course.Should().NotBeNull();
        course!.Status.Should().Be(CourseStatus.Draft);
        course.Title.Should().Be("Minimal C# Course");
    }

    [Fact]
    public async Task IncrementalSectionAndLessonCreation_WithoutVideo_ShouldSucceed()
    {
        // Arrange
        using var dbContext = CreateInMemoryDbContext();
        var instructorId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();

        var course = LearnHub.Domain.Courses.Course.CreateDraft(
            courseId,
            "Incremental Course",
            instructorId,
            categoryId).Value;

        dbContext.Courses.Add(course);
        await dbContext.SaveChangesAsync();

        var createSectionHandler = new CreateSectionCommandHandler(dbContext);
        var createLessonHandler = new CreateLessonCommandHandler(dbContext);

        // Act 1: Create Section
        var sectionResult = await createSectionHandler.Handle(
            new CreateSectionCommand(courseId, instructorId, "Section 1: Basics", "Introductory section"),
            CancellationToken.None);

        sectionResult.IsSuccess.Should().BeTrue();
        var sectionId = sectionResult.Value;

        // Act 2: Create Lesson without video
        var lessonResult = await createLessonHandler.Handle(
            new CreateLessonCommand(sectionId, instructorId, "Lesson 1.1: Hello World"),
            CancellationToken.None);

        // Assert
        lessonResult.IsSuccess.Should().BeTrue();
        var lesson = await dbContext.Lessons.FindAsync(lessonResult.Value);
        lesson.Should().NotBeNull();
        lesson!.Title.Should().Be("Lesson 1.1: Hello World");
        lesson.VideoUrl.Should().BeEmpty();
        lesson.IsPublished.Should().BeFalse();
    }

    [Fact]
    public async Task ReorderSectionsAndLessons_ShouldMaintainCorrectSequence()
    {
        // Arrange
        using var dbContext = CreateInMemoryDbContext();
        var instructorId = Guid.NewGuid();
        var courseId = Guid.NewGuid();

        var course = LearnHub.Domain.Courses.Course.CreateDraft(courseId, "Reorder Course", instructorId, Guid.NewGuid()).Value;
        dbContext.Courses.Add(course);
        await dbContext.SaveChangesAsync();

        var createSectionHandler = new CreateSectionCommandHandler(dbContext);
        var sec1 = (await createSectionHandler.Handle(new CreateSectionCommand(courseId, instructorId, "Section 1", "Desc 1"), CancellationToken.None)).Value;
        var sec2 = (await createSectionHandler.Handle(new CreateSectionCommand(courseId, instructorId, "Section 2", "Desc 2"), CancellationToken.None)).Value;

        var reorderHandler = new ReorderSectionsCommandHandler(dbContext);

        // Act: Swap section orders (sec1 -> order 2, sec2 -> order 1)
        var reorderResult = await reorderHandler.Handle(
            new ReorderSectionsCommand(courseId, instructorId, new List<SectionOrderItem>
            {
                new(sec1, 2),
                new(sec2, 1)
            }),
            CancellationToken.None);

        // Assert
        reorderResult.IsSuccess.Should().BeTrue();
        var section1 = await dbContext.Sections.FindAsync(sec1);
        var section2 = await dbContext.Sections.FindAsync(sec2);

        section1!.Order.Should().Be(2);
        section2!.Order.Should().Be(1);
    }

    [Fact]
    public async Task CourseReadinessCheck_ShouldFailWhenRequiredMetadataOrVideoIsMissing()
    {
        // Arrange
        using var dbContext = CreateInMemoryDbContext();
        var instructorId = Guid.NewGuid();
        var courseId = Guid.NewGuid();

        var course = LearnHub.Domain.Courses.Course.CreateDraft(courseId, "Readiness Course", instructorId, Guid.NewGuid()).Value;
        dbContext.Courses.Add(course);
        await dbContext.SaveChangesAsync();

        var readinessHandler = new GetCourseReadinessQueryHandler(dbContext);

        // Act: Check readiness on raw draft
        var readinessResult = await readinessHandler.Handle(new GetCourseReadinessQuery(courseId), CancellationToken.None);

        // Assert
        readinessResult.IsSuccess.Should().BeTrue();
        readinessResult.Value.CanPublish.Should().BeFalse();
        readinessResult.Value.Requirements.Should().Contain(r => r.Key == "thumbnail" && !r.IsValid);
        readinessResult.Value.Requirements.Should().Contain(r => r.Key == "sections" && !r.IsValid);
    }

    [Fact]
    public async Task StudentVisibility_ShouldFilterOutUnpublishedDraftSectionsAndLessons()
    {
        // Arrange
        using var dbContext = CreateInMemoryDbContext();
        var instructorId = Guid.NewGuid();
        var studentId = Guid.NewGuid();
        var courseId = Guid.NewGuid();

        var course = LearnHub.Domain.Courses.Course.CreateDraft(courseId, "Visibility Course", instructorId, Guid.NewGuid()).Value;
        dbContext.Courses.Add(course);

        // Published Section 1 with 1 Published Lesson and 1 Draft Lesson
        var sec1 = LearnHub.Domain.Courses.Sections.Section.Create(Guid.NewGuid(), "Section 1", "Desc", 1, courseId, isPublished: true).Value;
        var les1Published = LearnHub.Domain.Courses.Sections.Lessons.Lesson.Create(Guid.NewGuid(), "Lesson 1", "Desc", "https://video.mp4", false, "Content", 10, 1, sec1.Id, isPublished: true).Value;
        var les2Draft = LearnHub.Domain.Courses.Sections.Lessons.Lesson.Create(Guid.NewGuid(), "Lesson 2 Draft", "Desc", "https://video.mp4", false, "Content", 10, 2, sec1.Id, isPublished: false).Value;

        // Draft Section 2
        var sec2Draft = LearnHub.Domain.Courses.Sections.Section.Create(Guid.NewGuid(), "Section 2 Draft", "Desc", 2, courseId, isPublished: false).Value;

        dbContext.Sections.AddRange(sec1, sec2Draft);
        dbContext.Lessons.AddRange(les1Published, les2Draft);
        await dbContext.SaveChangesAsync();

        // Student Mock Context
        var mockStudentUserService = new Mock<ICurrentUserService>();
        mockStudentUserService.Setup(s => s.UserId).Returns(studentId);

        var studentQueryHandler = new GetCourseContentQueryHandler(dbContext, mockStudentUserService.Object);

        // Act: Fetch content as student
        var studentResult = await studentQueryHandler.Handle(new GetCourseContentQuery(courseId), CancellationToken.None);

        // Assert Student Visibility
        studentResult.IsSuccess.Should().BeTrue();
        studentResult.Value.Sections.Should().HaveCount(1); // Only Section 1
        studentResult.Value.Sections.First().Lessons.Should().HaveCount(1); // Only Lesson 1
        studentResult.Value.Sections.First().Lessons.First().Title.Should().Be("Lesson 1");

        // Instructor Mock Context
        var mockInstructorUserService = new Mock<ICurrentUserService>();
        mockInstructorUserService.Setup(s => s.UserId).Returns(instructorId);

        var instructorQueryHandler = new GetCourseContentQueryHandler(dbContext, mockInstructorUserService.Object);

        // Act: Fetch content as instructor
        var instructorResult = await instructorQueryHandler.Handle(new GetCourseContentQuery(courseId), CancellationToken.None);

        // Assert Instructor Visibility
        instructorResult.IsSuccess.Should().BeTrue();
        instructorResult.Value.Sections.Should().HaveCount(2); // Both Sections 1 and 2
        instructorResult.Value.Sections.First().Lessons.Should().HaveCount(2); // Both Lessons 1 and 2
    }
}
