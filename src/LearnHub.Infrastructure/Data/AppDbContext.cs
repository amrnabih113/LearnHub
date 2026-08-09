using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Common.Interfaces;
using LearnHub.Domain.Classification.Categories;
using LearnHub.Domain.Classification.Tags;
using LearnHub.Domain.Common;
using LearnHub.Domain.Courses;
using LearnHub.Domain.Courses.Sections;
using LearnHub.Domain.Courses.Sections.Lessons;
using LearnHub.Domain.Courses.Sections.Lessons.Resources;
using LearnHub.Domain.Enrollments;
using LearnHub.Domain.Identity;
using LearnHub.Domain.Purchasing.Carts;
using LearnHub.Domain.Purchasing.Coupons;
using LearnHub.Domain.Purchasing.Orders;
using LearnHub.Domain.Purchasing.Payments;
using LearnHub.Domain.Reviews.CourseReviews;
using LearnHub.Domain.Reviews.InstructorReviews;
using LearnHub.Domain.Subscriptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Infrastructure.Data;

public class AppDbContext(
    DbContextOptions<AppDbContext> options,
    IMediator? mediator = null)
    : DbContext(options), IAppDbContext
{
    private readonly IMediator? _mediator = mediator;

    private bool _dispatchingDomainEvents;

    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Section> Sections => Set<Section>();
    public DbSet<Lesson> Lessons => Set<Lesson>();
    public DbSet<Resource> Resources => Set<Resource>();

    public DbSet<Enrollment> Enrollments => Set<Enrollment>();

    public DbSet<User> Users => Set<User>();

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();

    public DbSet<OtpCode> OtpCodes => Set<OtpCode>();

    public DbSet<Cart> Carts => Set<Cart>();

    public DbSet<Order> Orders => Set<Order>();

    public DbSet<Payment> Payments => Set<Payment>();

    public DbSet<Coupon> Coupons => Set<Coupon>();

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Tag> Tags => Set<Tag>();

    public DbSet<CourseReview> CourseReviews => Set<CourseReview>();

    public DbSet<InstructorReview> InstructorReviews => Set<InstructorReview>();

    public DbSet<Subscription> Subscriptions => Set<Subscription>();

    public DbSet<SubscriptionPlan> SubscriptionPlans => Set<SubscriptionPlan>();

    public DbSet<SubscriptionPayment> SubscriptionPayments => Set<SubscriptionPayment>();



    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(
            typeof(AppDbContext).Assembly);
    }


    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        var result = await base.SaveChangesAsync(cancellationToken);

        await DispatchDomainEventsAsync(cancellationToken);

        return result;
    }


    private async Task DispatchDomainEventsAsync(
        CancellationToken cancellationToken)
    {
        if (_mediator is null || _dispatchingDomainEvents)
            return;


        _dispatchingDomainEvents = true;

        try
        {
            var entities = ChangeTracker
            .Entries<Entity>()
            .Where(x => x.Entity.DomainEvents.Count != 0)
            .Select(x => x.Entity)
            .ToList();


            var events = entities
                .SelectMany(x => x.DomainEvents)
                .ToList();

            foreach (var entity in entities)
            {
                entity.ClearDomainEvents();
            }


            foreach (var domainEvent in events)
            {
                await _mediator.Publish(
                    domainEvent,
                    cancellationToken);
            }
        }
        finally
        {
            _dispatchingDomainEvents = false;
        }
    }
}

