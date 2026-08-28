using FluentAssertions;
using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Enrollments.Dtos;
using LearnHub.Application.Features.Search.Dtos;
using LearnHub.Application.Features.Search.Queries.SearchAutoComplete;
using LearnHub.Application.Features.Search.Queries.SearchCourses;
using LearnHub.Application.Features.Search.Services;
using LearnHub.Domain.Classification.Categories;
using LearnHub.Domain.Classification.Tags;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Courses;
using LearnHub.Domain.Courses.Enums;
using LearnHub.Domain.Identity;
using LearnHub.Domain.Purchasing.ValueObjects;
using LearnHub.Domain.Subscriptions;
using LearnHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace LearnHub.UnitTests;

public class SearchTests
{
    private static AppDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public void Normalizer_ShouldCleanAndTokenizeInput()
    {
        // Arrange
        var normalizer = new SearchQueryNormalizer();

        // Act
        var normalized = normalizer.Normalize("  ASP.NET  Core   ");
        var tokens = normalizer.Tokenize("ASP.NET Core 10");
        var synonyms = normalizer.ExpandSynonyms("c#");

        // Assert
        normalized.Should().Be("asp.net core");
        tokens.Should().HaveCount(3);
        synonyms.Should().Contain("dotnet");
    }

    [Fact]
    public async Task AutoComplete_ShouldReturnMatchingSuggestions()
    {
        // Arrange
        using var dbContext = CreateInMemoryDbContext();
        var category = Category.Create(Guid.NewGuid(), "Software Engineering", "software-engineering", "Desc").Value;
        dbContext.Categories.Add(category);

        var course = Course.Create(
            Guid.NewGuid(), "ASP.NET Core Architecture", "Learn Clean Architecture",
            Guid.NewGuid(), category.Id, null, CourseLevel.Advanced, CourseStatus.Published,
            Money.Create(100, "USD").Value, false, SubscriptionTier.Free, "en", "English", "US").Value;
        dbContext.Courses.Add(course);

        await dbContext.SaveChangesAsync();

        var normalizer = new SearchQueryNormalizer();
        var handler = new SearchAutoCompleteQueryHandler(dbContext, normalizer);

        // Act
        var result = await handler.Handle(new SearchAutoCompleteQuery("asp.net"), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.CourseSuggestions.Should().HaveCount(1);
        result.Value.CourseSuggestions[0].Text.Should().Be("ASP.NET Core Architecture");
    }

    [Fact]
    public async Task SearchCourses_WithFiltersAndPagination_ShouldReturnFilteredPagedResult()
    {
        // Arrange
        using var dbContext = CreateInMemoryDbContext();
        var category = Category.Create(Guid.NewGuid(), "Backend Development", "backend-dev", "Desc").Value;
        dbContext.Categories.Add(category);

        var course1 = Course.Create(
            Guid.NewGuid(), "Mastering C# 12", "Complete C# course",
            Guid.NewGuid(), category.Id, null, CourseLevel.Beginner, CourseStatus.Published,
            Money.Create(0, "USD").Value, true, SubscriptionTier.Free, "en", "English", "US").Value;

        var course2 = Course.Create(
            Guid.NewGuid(), "Advanced Entity Framework", "EF Core performance",
            Guid.NewGuid(), category.Id, null, CourseLevel.Advanced, CourseStatus.Published,
            Money.Create(150, "USD").Value, false, SubscriptionTier.Pro, "en", "English", "US").Value;

        dbContext.Courses.AddRange(course1, course2);
        await dbContext.SaveChangesAsync();

        var normalizer = new SearchQueryNormalizer();
        var accessServiceMock = new Mock<ICourseAccessService>();

        var handler = new SearchCoursesQueryHandler(dbContext, normalizer, accessServiceMock.Object);

        // Act - Filter Free Courses
        var query = new SearchCoursesQuery(IsFree: true);
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().HaveCount(1);
        result.Value.Items.First().Title.Should().Be("Mastering C# 12");
        result.Value.Items.First().IsFree.Should().BeTrue();
    }

    [Fact]
    public void FuzzyMatcher_ShouldCalculateSimilarityAndDistance()
    {
        // Arrange
        var matcher = new FuzzyMatcher();

        // Act
        int distance = matcher.LevenshteinDistance("javascript", "javscript");
        double similarity = matcher.CalculateSimilarity("javascript", "javscript");

        // Assert
        distance.Should().Be(1);
        similarity.Should().BeGreaterThan(0.85);
    }

    [Fact]
    public void SynonymProvider_ShouldReturnExpandedSynonyms()
    {
        // Arrange
        var provider = new SynonymProvider();

        // Act
        var synonyms = provider.GetSynonyms("js");

        // Assert
        synonyms.Should().Contain("javascript");
    }

    [Fact]
    public void RankingService_ShouldCalculateWeightedFinalScore()
    {
        // Arrange
        var rankingService = new SearchRankingService();
        var candidate = new Application.Features.Search.Models.SearchCandidate(
            CourseId: Guid.NewGuid(),
            ExactScore: 100,
            FullTextScore: 80,
            FuzzyScore: 50,
            SynonymScore: 50,
            SemanticScore: 0,
            RatingScore: 5,
            PopularityScore: 10);

        // Act
        var calculated = rankingService.CalculateFinalScore(candidate);

        // Assert
        calculated.FinalScore.Should().BeGreaterThan(0);
    }
}
