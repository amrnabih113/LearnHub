using FluentAssertions;
using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Common.Interfaces.Authentication;
using LearnHub.Application.Features.Assessments.EventHandlers;
using LearnHub.Application.Features.Courses.Queries.GetCourseContent;
using LearnHub.Application.Features.Enrollments.Dtos;
using LearnHub.Application.Features.Quizzes.Commands.AddQuestion;
using LearnHub.Application.Features.Quizzes.Commands.CreateFinalExam;
using LearnHub.Application.Features.Quizzes.Commands.CreateSectionQuiz;
using LearnHub.Application.Features.Quizzes.Commands.PublishQuiz;
using LearnHub.Application.Features.Quizzes.Commands.SaveQuizAnswer;
using LearnHub.Application.Features.Quizzes.Commands.StartQuizAttempt;
using LearnHub.Application.Features.Quizzes.Commands.SubmitQuizAttempt;
using LearnHub.Domain.Assessments;
using LearnHub.Domain.Assessments.Enums;
using LearnHub.Domain.Assessments.Events;
using LearnHub.Domain.Assessments.Questions;
using LearnHub.Domain.Classification.Categories;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Courses;
using LearnHub.Domain.Courses.Enums;
using LearnHub.Domain.Courses.Sections;
using LearnHub.Domain.Courses.Sections.Lessons;
using LearnHub.Domain.Enrollments;
using LearnHub.Domain.Enrollments.Enums;
using LearnHub.Domain.Identity;
using LearnHub.Domain.Purchasing.ValueObjects;
using LearnHub.Domain.Subscriptions;
using LearnHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace LearnHub.UnitTests;

public class AssessmentSystemTests
{
    private static AppDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task CreateSectionQuizAndPublish_ShouldSucceed()
    {
        // Arrange
        using var dbContext = CreateInMemoryDbContext();
        var courseId = Guid.NewGuid();
        var sectionId = Guid.NewGuid();

        var category = Category.Create(Guid.NewGuid(), "Backend", "backend", "Desc").Value;
        dbContext.Categories.Add(category);

        var course = Course.Create(
            courseId, "C# Fundamentals", "Desc",
            Guid.NewGuid(), category.Id, null, CourseLevel.Beginner, CourseStatus.Published,
            Money.Create(50, "USD").Value, false, SubscriptionTier.Free, "en", "English", "US").Value;

        var section = Section.Create(sectionId, "Section 1", "Desc", 1, courseId).Value;
        course.UpsertSections([section]);
        dbContext.Courses.Add(course);
        await dbContext.SaveChangesAsync();

        var createHandler = new CreateSectionQuizCommandHandler(dbContext);
        var createCommand = new CreateSectionQuizCommand(
            courseId, sectionId, "Section 1 Quiz", "Test description", 15, 3, 70);

        // Act - Create Quiz
        var createResult = await createHandler.Handle(createCommand, CancellationToken.None);
        createResult.IsSuccess.Should().BeTrue();
        var quizId = createResult.Value;
        dbContext.ChangeTracker.Clear();

        // Add Question
        var questionHandler = new AddQuestionCommandHandler(dbContext);
        var addQuestionCommand = new AddQuestionCommand(
            quizId,
            "What is C#?",
            QuestionType.MultipleChoice,
            10,
            1,
            [new ChoiceInput("Programming Language", true), new ChoiceInput("Fruit", false)]);

        var questionResult = await questionHandler.Handle(addQuestionCommand, CancellationToken.None);
        questionResult.IsSuccess.Should().BeTrue();
        dbContext.ChangeTracker.Clear();

        // Publish Quiz
        var publishHandler = new PublishQuizCommandHandler(dbContext);
        var publishResult = await publishHandler.Handle(new PublishQuizCommand(quizId), CancellationToken.None);
        dbContext.ChangeTracker.Clear();

        // Assert
        publishResult.IsSuccess.Should().BeTrue();
        var quizInDb = await dbContext.Quizzes.FirstOrDefaultAsync(q => q.Id == quizId);
        quizInDb.Should().NotBeNull();
        quizInDb!.Status.Should().Be(QuizStatus.Published);
        quizInDb.Type.Should().Be(QuizType.Section);
        quizInDb.SectionId.Should().Be(sectionId);
    }

    [Fact]
    public async Task PointsBasedWeightedGrading_ShouldCalculateEarnedPointsCorrectly()
    {
        // Arrange
        var quiz = Quiz.CreateSectionQuiz(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Quiz", "Desc", 20, 3, 70).Value;

        var q1Id = Guid.NewGuid();
        var c1CorrectId = Guid.NewGuid();
        quiz.AddQuestion(q1Id, "Q1", QuestionType.MultipleChoice, 2, 1);
        quiz.AddChoice(q1Id, c1CorrectId, "Correct Choice", true);
        quiz.AddChoice(q1Id, Guid.NewGuid(), "Wrong Choice", false);

        var q2Id = Guid.NewGuid();
        var c2CorrectId = Guid.NewGuid();
        quiz.AddQuestion(q2Id, "Q2", QuestionType.MultipleChoice, 8, 2);
        quiz.AddChoice(q2Id, c2CorrectId, "Correct Choice 2", true);
        quiz.AddChoice(q2Id, Guid.NewGuid(), "Wrong Choice 2", false);

        quiz.Publish();

        // Total points = 10. Student answers Q2 correctly (8 pts) and Q1 wrongly (0 pts).
        var attempt = quiz.StartAttempt(Guid.NewGuid(), Guid.NewGuid(), 0).Value;
        attempt.AnswerQuestion(q1Id, Domain.Assessments.ValueObjects.AnswerOption.FromChoice(Guid.NewGuid()).Value, DateTimeOffset.UtcNow);
        attempt.AnswerQuestion(q2Id, Domain.Assessments.ValueObjects.AnswerOption.FromChoice(c2CorrectId).Value, DateTimeOffset.UtcNow);

        // Act
        var submitResult = attempt.Submit(quiz, DateTimeOffset.UtcNow);

        // Assert
        submitResult.IsSuccess.Should().BeTrue();
        attempt.ScorePercentage.Should().Be(80m); // 8 / 10 * 100 = 80%
        attempt.Grade!.IsPassed.Should().BeTrue();
    }

    [Fact]
    public async Task RetakeFlow_FailedAttemptCanBeRetakenUntilMaxAttemptsExceeded()
    {
        // Arrange
        using var dbContext = CreateInMemoryDbContext();
        var studentId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var sectionId = Guid.NewGuid();

        var quiz = Quiz.CreateSectionQuiz(
            Guid.NewGuid(), courseId, sectionId, "Gated Quiz", "Desc", 30, 2, 70).Value; // Max 2 attempts

        var qId = Guid.NewGuid();
        var correctChoiceId = Guid.NewGuid();
        var wrongChoiceId = Guid.NewGuid();
        quiz.AddQuestion(qId, "Question 1", QuestionType.MultipleChoice, 10, 1);
        quiz.AddChoice(qId, correctChoiceId, "Correct", true);
        quiz.AddChoice(qId, wrongChoiceId, "Wrong", false);
        quiz.Publish();

        dbContext.Quizzes.Add(quiz);
        await dbContext.SaveChangesAsync();

        var accessServiceMock = new Mock<ICourseAccessService>();
        accessServiceMock.Setup(x => x.EvaluateAccessAsync(studentId, courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CourseAccessResult(courseId, studentId, true, true, true, EnrollmentStatus.Active, 0, new CourseEntitlementsDto(true, false, false, false, false)));

        var startHandler = new StartQuizAttemptCommandHandler(dbContext, accessServiceMock.Object);
        var saveHandler = new SaveQuizAnswerCommandHandler(dbContext);
        var submitHandler = new SubmitQuizAttemptCommandHandler(dbContext);

        // Attempt 1: Answer wrongly -> Fail
        var startResult1 = await startHandler.Handle(new StartQuizAttemptCommand(quiz.Id, studentId), CancellationToken.None);
        startResult1.IsSuccess.Should().BeTrue();
        dbContext.ChangeTracker.Clear();

        await saveHandler.Handle(new SaveQuizAnswerCommand(startResult1.Value.AttemptId, qId, studentId, wrongChoiceId), CancellationToken.None);
        dbContext.ChangeTracker.Clear();

        var submitResult1 = await submitHandler.Handle(new SubmitQuizAttemptCommand(startResult1.Value.AttemptId, studentId), CancellationToken.None);
        submitResult1.IsSuccess.Should().BeTrue();
        submitResult1.Value.IsPassed.Should().BeFalse();
        submitResult1.Value.AttemptsRemaining.Should().Be(1);
        dbContext.ChangeTracker.Clear();

        // Attempt 2: Answer correctly -> Pass
        var startResult2 = await startHandler.Handle(new StartQuizAttemptCommand(quiz.Id, studentId), CancellationToken.None);
        startResult2.IsSuccess.Should().BeTrue();
        dbContext.ChangeTracker.Clear();

        await saveHandler.Handle(new SaveQuizAnswerCommand(startResult2.Value.AttemptId, qId, studentId, correctChoiceId), CancellationToken.None);
        dbContext.ChangeTracker.Clear();

        var submitResult2 = await submitHandler.Handle(new SubmitQuizAttemptCommand(startResult2.Value.AttemptId, studentId), CancellationToken.None);
        submitResult2.IsSuccess.Should().BeTrue();
        submitResult2.Value.IsPassed.Should().BeTrue();
        dbContext.ChangeTracker.Clear();

        // Attempt 3: Max attempts exceeded
        var startResult3 = await startHandler.Handle(new StartQuizAttemptCommand(quiz.Id, studentId), CancellationToken.None);
        startResult3.IsError.Should().BeTrue();
        startResult3.TopError.Code.Should().Be(QuizErrors.MaxAttemptsExceeded.Code);
    }

    [Fact]
    public async Task FinalExamPassing_ShouldCompleteEnrollment()
    {
        // Arrange
        using var dbContext = CreateInMemoryDbContext();
        var studentId = Guid.NewGuid();
        var courseId = Guid.NewGuid();

        var category = Category.Create(Guid.NewGuid(), "Cloud", "cloud", "Desc").Value;
        dbContext.Categories.Add(category);

        var course = Course.Create(
            courseId, "Azure Architecture", "Desc",
            Guid.NewGuid(), category.Id, null, CourseLevel.Advanced, CourseStatus.Published,
            Money.Create(100, "USD").Value, false, SubscriptionTier.Free, "en", "English", "US").Value;
        dbContext.Courses.Add(course);

        var enrollment = Enrollment.Create(Guid.NewGuid(), studentId, courseId).Value;
        dbContext.Enrollments.Add(enrollment);

        var finalExam = Quiz.CreateFinalExam(
            Guid.NewGuid(), courseId, "Final Exam", "Desc", 60, 3, 70).Value;
        dbContext.Quizzes.Add(finalExam);

        await dbContext.SaveChangesAsync();

        var eventHandler = new QuizPassedDomainEventHandler(dbContext, NullLogger<QuizPassedDomainEventHandler>.Instance);

        // Act - Simulate QuizPassedDomainEvent for Final Exam
        await eventHandler.Handle(new QuizPassedDomainEvent(Guid.NewGuid(), finalExam.Id, studentId, 85m), CancellationToken.None);

        // Assert
        var updatedEnrollment = await dbContext.Enrollments.FirstOrDefaultAsync(e => e.Id == enrollment.Id);
        updatedEnrollment.Should().NotBeNull();
        updatedEnrollment!.Status.Should().Be(EnrollmentStatus.Completed);
        updatedEnrollment.CompletedAtUtc.Should().NotBeNull();
    }

    [Fact]
    public async Task CourseContentQuery_ShouldIncludeAssessmentsAndGatingStatus()
    {
        // Arrange
        using var dbContext = CreateInMemoryDbContext();
        var studentId = Guid.NewGuid();
        var courseId = Guid.NewGuid();

        var category = Category.Create(Guid.NewGuid(), "DevOps", "devops", "Desc").Value;
        dbContext.Categories.Add(category);

        var course = Course.Create(
            courseId, "Kubernetes Mastery", "Desc",
            Guid.NewGuid(), category.Id, null, CourseLevel.Advanced, CourseStatus.Published,
            Money.Create(150, "USD").Value, false, SubscriptionTier.Free, "en", "English", "US").Value;

        var section1 = Section.Create(Guid.NewGuid(), "Section 1", "Desc", 1, courseId).Value;
        var section2 = Section.Create(Guid.NewGuid(), "Section 2", "Desc", 2, courseId).Value;

        course.UpsertSections([section1, section2]);
        dbContext.Courses.Add(course);

        var quizSection1 = Quiz.CreateSectionQuiz(
            Guid.NewGuid(), courseId, section1.Id, "Quiz 1", "Desc", 15, 3, 70).Value;
        var qId = Guid.NewGuid();
        quizSection1.AddQuestion(qId, "Q1", QuestionType.MultipleChoice, 10, 1);
        quizSection1.AddChoice(qId, Guid.NewGuid(), "Correct Answer", true);
        quizSection1.Publish();

        dbContext.Quizzes.Add(quizSection1);
        await dbContext.SaveChangesAsync();

        var currentUserMock = new Mock<ICurrentUserService>();
        currentUserMock.Setup(x => x.UserId).Returns(studentId);

        var queryHandler = new GetCourseContentQueryHandler(dbContext, currentUserMock.Object);

        // Act
        var result = await queryHandler.Handle(new GetCourseContentQuery(courseId), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Sections.Should().HaveCount(2);

        var sec1Dto = result.Value.Sections.First(s => s.Id == section1.Id);
        sec1Dto.Assessment.Should().NotBeNull();
        sec1Dto.Assessment!.Status.Should().Be("Available");
        sec1Dto.IsLocked.Should().BeFalse();

        var sec2Dto = result.Value.Sections.First(s => s.Id == section2.Id);
        sec2Dto.IsLocked.Should().BeTrue(); // Locked because Section 1 Quiz has not been passed
    }
}
