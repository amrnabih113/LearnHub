using LearnHub.Domain.Classification.Categories;
using LearnHub.Domain.Classification.Tags;
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
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.common.Interfaces;

public interface IAppDbContext
{

    public DbSet<Course> Courses { get; }
    public DbSet<Section> Sections { get; }
    public DbSet<Lesson> Lessons { get; }
    public DbSet<Resource> Resources { get; }

    public DbSet<Enrollment> Enrollments { get; }

    public DbSet<User> Users { get; }
    public DbSet<RefreshToken> RefreshTokens { get; }
    public DbSet<PasswordResetToken> PasswordResetTokens { get; }
    public DbSet<OtpCode> OtpCodes { get; }
    public DbSet<Cart> Carts { get; }

    public DbSet<Order> Orders { get; }

    public DbSet<Payment> Payments { get; }

    public DbSet<Coupon> Coupons { get; }

    public DbSet<Category> Categories { get; }

    public DbSet<Tag> Tags { get; }

    public DbSet<CourseReview> CourseReviews { get; }

    public DbSet<InstructorReview> InstructorReviews { get; }

    public DbSet<Subscription> Subscriptions { get; }

    public DbSet<SubscriptionPlan> SubscriptionPlans { get; }

    public DbSet<SubscriptionPayment> SubscriptionPayments { get; }


    Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}