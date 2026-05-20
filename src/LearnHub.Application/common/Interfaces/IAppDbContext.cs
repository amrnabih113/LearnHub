using LearnHub.Domain.Classification.Categories;
using LearnHub.Domain.Classification.Tags;
using LearnHub.Domain.Courses;
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

    DbSet<Course> Courses { get; }

    DbSet<Enrollment> Enrollments { get; }

    DbSet<User> Users { get; }

    DbSet<Cart> Carts { get; }

    DbSet<Order> Orders { get; }

    DbSet<Payment> Payments { get; }

    DbSet<Coupon> Coupons { get; }

    DbSet<Category> Categories { get; }

    DbSet<Tag> Tags { get; }

    DbSet<CourseReview> CourseReviews { get; }

    DbSet<InstructorReview> InstructorReviews { get; }

    DbSet<Subscription> Subscriptions { get; }

    DbSet<SubscriptionPlan> SubscriptionPlans { get; }

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}