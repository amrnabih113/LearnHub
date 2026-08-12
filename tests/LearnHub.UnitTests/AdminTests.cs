using FluentAssertions;
using LearnHub.Application.Features.Admin.Commands.CreateCategory;
using LearnHub.Application.Features.Admin.Commands.CreateTag;
using LearnHub.Application.Features.Admin.Commands.DeleteCategory;
using LearnHub.Application.Features.Admin.Commands.DeleteTag;
using LearnHub.Application.Features.Admin.Commands.UpdateCategory;
using LearnHub.Application.Features.Admin.Commands.UpdateTag;
using LearnHub.Application.Features.Admin.Queries.GetAdminDashboard;
using LearnHub.Application.Features.Admin.Queries.GetCategoriesAdmin;
using LearnHub.Application.Features.Admin.Queries.GetTagsAdmin;
using LearnHub.Application.Features.Admin.Queries.GetUsersAdmin;
using LearnHub.Domain.Classification.Categories;
using LearnHub.Domain.Classification.Tags;
using LearnHub.Domain.Identity;
using LearnHub.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace LearnHub.UnitTests;

public class AdminTests
{
    private readonly DbContextOptions<AppDbContext> _dbOptions;
    private readonly Mock<IMediator> _mediatorMock;

    public AdminTests()
    {
        _dbOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _mediatorMock = new Mock<IMediator>();
    }

    private AppDbContext CreateDbContext() => new AppDbContext(_dbOptions, _mediatorMock.Object);

    [Fact]
    public async Task CreateCategory_WhenValid_ShouldSucceed()
    {
        using var context = CreateDbContext();
        var handler = new CreateCategoryCommandHandler(context);
        var command = new CreateCategoryCommand("Programming", "programming", "Software development courses");

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsError.Should().BeFalse();
        result.Value.Name.Should().Be("Programming");
        result.Value.Slug.Should().Be("programming");

        var categoryInDb = await context.Categories.FirstOrDefaultAsync(c => c.Id == result.Value.Id);
        categoryInDb.Should().NotBeNull();
        categoryInDb!.Name.Should().Be("Programming");
    }

    [Fact]
    public async Task CreateCategory_WhenDuplicateName_ShouldReturnError()
    {
        using var context = CreateDbContext();
        var category = Category.Create(Guid.NewGuid(), "Design", "design").Value;
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var handler = new CreateCategoryCommandHandler(context);
        var command = new CreateCategoryCommand("Design", "design-new");

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsError.Should().BeTrue();
        result.Errors[0].Code.Should().Be("DomainError.Category.DuplicateName");
    }

    [Fact]
    public async Task UpdateCategory_WhenCircularParentDependency_ShouldReturnError()
    {
        using var context = CreateDbContext();
        var cat1 = Category.Create(Guid.NewGuid(), "Web Development", "web-dev").Value;
        context.Categories.Add(cat1);
        await context.SaveChangesAsync();

        var cat2 = Category.Create(Guid.NewGuid(), "Frontend", "frontend", parentCategoryId: cat1.Id).Value;
        context.Categories.Add(cat2);
        await context.SaveChangesAsync();

        // Try setting parent of cat1 to cat2 (creating a cycle)
        var handler = new UpdateCategoryCommandHandler(context);
        var command = new UpdateCategoryCommand(cat1.Id, "Web Development", "web-dev", null, cat2.Id);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsError.Should().BeTrue();
        result.Errors[0].Code.Should().Be("DomainError.Category.HierarchyInvalid");
    }

    [Fact]
    public async Task DeleteCategory_WhenHasSubcategories_ShouldReturnError()
    {
        using var context = CreateDbContext();
        var parent = Category.Create(Guid.NewGuid(), "Backend", "backend").Value;
        context.Categories.Add(parent);
        await context.SaveChangesAsync();

        var child = Category.Create(Guid.NewGuid(), "C#", "csharp", parentCategoryId: parent.Id).Value;
        context.Categories.Add(child);
        await context.SaveChangesAsync();

        var handler = new DeleteCategoryCommandHandler(context);
        var command = new DeleteCategoryCommand(parent.Id);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsError.Should().BeTrue();
        result.Errors[0].Code.Should().Be("DomainError.Category.HasSubcategories");
    }

    [Fact]
    public async Task CreateTag_WhenValid_ShouldSucceed()
    {
        using var context = CreateDbContext();
        var handler = new CreateTagCommandHandler(context);
        var command = new CreateTagCommand("csharp", "csharp", "C# Language");

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsError.Should().BeFalse();
        result.Value.Name.Should().Be("csharp");

        var tagInDb = await context.Tags.FirstOrDefaultAsync(t => t.Id == result.Value.Id);
        tagInDb.Should().NotBeNull();
    }

    [Fact]
    public async Task GetUsersAdmin_WhenFilteredByRole_ShouldReturnOnlyMatchingUsers()
    {
        using var context = CreateDbContext();
        var admin = User.Create(Guid.NewGuid(), "Admin", "User", "admin@learnhub.com", "secret_hash_1", Role.Admin).Value;
        var student = User.Create(Guid.NewGuid(), "Student", "User", "student@learnhub.com", "secret_hash_2", Role.Student).Value;

        context.Users.AddRange(admin, student);
        await context.SaveChangesAsync();

        var handler = new GetUsersAdminQueryHandler(context);
        var query = new GetUsersAdminQuery(Role: "Admin");

        var result = await handler.Handle(query, CancellationToken.None);

        result.IsError.Should().BeFalse();
        result.Value.Items.Should().HaveCount(1);
        result.Value.Items.First().Email.Should().Be("admin@learnhub.com");
    }

    [Fact]
    public async Task GetAdminDashboard_ShouldReturnAggregatedMetrics()
    {
        using var context = CreateDbContext();
        var admin = User.Create(Guid.NewGuid(), "System", "Admin", "sysadmin@learnhub.com", "hash", Role.Admin).Value;
        var student = User.Create(Guid.NewGuid(), "John", "Doe", "john@learnhub.com", "hash", Role.Student).Value;
        context.Users.AddRange(admin, student);
        await context.SaveChangesAsync();

        var handler = new GetAdminDashboardQueryHandler(context);
        var query = new GetAdminDashboardQuery("AllTime");

        var result = await handler.Handle(query, CancellationToken.None);

        result.IsError.Should().BeFalse();
        result.Value.Users.TotalUsers.Should().Be(2);
        result.Value.Users.Admins.Should().Be(1);
        result.Value.Users.Students.Should().Be(1);
    }
}
