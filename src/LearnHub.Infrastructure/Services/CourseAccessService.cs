using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Enrollments.Dtos;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Courses;
using LearnHub.Domain.Enrollments;
using LearnHub.Domain.Enrollments.Enums;
using LearnHub.Domain.Identity;
using LearnHub.Domain.Purchasing.Enums;
using LearnHub.Domain.Subscriptions;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Infrastructure.Services;

public sealed class CourseAccessService(IAppDbContext context) : ICourseAccessService
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<CourseAccessResult>> EvaluateAccessAsync(
        Guid studentId,
        Guid courseId,
        CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .Include(u => u.Roles)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == studentId, cancellationToken);

        if (user is null)
        {
            return Error.NotFound("CourseAccess.UserNotFound", "User not found.");
        }

        var course = await _context.Courses
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == courseId, cancellationToken);

        if (course is null)
        {
            return Error.NotFound("CourseAccess.CourseNotFound", "Course not found.");
        }

        var isAdminOrInstructor = user.Roles.Any(r => r.Role is Role.Admin or Role.Instructor);

        var isFreeCourse = course.Price.Amount == 0;

        var hasPurchase = await _context.Orders
            .AsNoTracking()
            .AnyAsync(o => o.StudentId == studentId
                        && o.Status == OrderStatus.Paid
                        && o.Items.Any(i => i.CourseId == courseId), cancellationToken);

        var activeSubscription = await _context.Subscriptions
            .Include(s => s.Plan)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.StudentId == studentId
                                   && (s.Status == SubscriptionStatus.Active || s.Status == SubscriptionStatus.Trialing)
                                   && s.ExpiresAtUtc > DateTimeOffset.UtcNow, cancellationToken);

        var activePlanTier = activeSubscription?.Plan?.Tier ?? activeSubscription?.Tier ?? SubscriptionTier.Free;

        var hasValidSubscription = course.IsIncludedInSubscription
            && activeSubscription is not null
            && activePlanTier >= course.RequiredSubscriptionTier;


        var enrollment = await _context.Enrollments
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId, cancellationToken);

        var isCompleted = enrollment is not null && enrollment.ProgressPercentage >= 100m;
        var canViewCertificate = isCompleted || (enrollment?.Certificate is not null);

        var isAccessible = isAdminOrInstructor
            || isFreeCourse
            || hasPurchase
            || hasValidSubscription;

        var canWatchLessons = isAccessible && (enrollment is null || enrollment.Status == EnrollmentStatus.Active || isFreeCourse || isAdminOrInstructor);

        var entitlements = new CourseEntitlementsDto(
            HasPurchase: hasPurchase,
            HasValidSubscription: hasValidSubscription,
            IsFreeCourse: isFreeCourse,
            IsAdminGranted: isAdminOrInstructor,
            IsCompleted: isCompleted);

        return new CourseAccessResult(
            CourseId: courseId,
            StudentId: studentId,
            IsAccessible: isAccessible,
            CanWatchLessons: canWatchLessons,
            CanViewCertificate: canViewCertificate,
            Status: enrollment?.Status,
            ProgressPercentage: enrollment?.ProgressPercentage ?? 0m,
            Entitlements: entitlements);
    }

    public async Task<Result<Updated>> SynchronizeUserEnrollmentsAsync(
        Guid studentId,
        CancellationToken cancellationToken = default)
    {
        var enrollments = await _context.Enrollments
            .Where(e => e.StudentId == studentId)
            .ToListAsync(cancellationToken);

        foreach (var enrollment in enrollments)
        {
            var accessResult = await EvaluateAccessAsync(studentId, enrollment.CourseId, cancellationToken);
            if (accessResult.IsError)
            {
                continue;
            }

            var access = accessResult.Value;

            if (enrollment.Status == EnrollmentStatus.Completed)
            {
                continue;
            }

            if (!access.IsAccessible && enrollment.Status == EnrollmentStatus.Active)
            {
                enrollment.Cancel();
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Updated;
    }

    public async Task<Result<Guid>> EnsureEnrollmentForCourseAccessAsync(
        Guid studentId,
        Guid courseId,
        CancellationToken cancellationToken = default)
    {
        var accessResult = await EvaluateAccessAsync(studentId, courseId, cancellationToken);
        if (accessResult.IsError)
        {
            return accessResult.Errors;
        }

        var access = accessResult.Value;
        if (!access.IsAccessible)
        {
            return Error.Validation("CourseAccess.Denied", "You do not have entitlement or payment to access this course.");
        }

        var enrollment = await _context.Enrollments
            .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId, cancellationToken);

        if (enrollment is null)
        {
            var createResult = Enrollment.Create(Guid.NewGuid(), studentId, courseId);
            if (createResult.IsError)
            {
                return createResult.Errors;
            }

            enrollment = createResult.Value;
            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync(cancellationToken);
            return enrollment.Id;
        }

        return enrollment.Id;
    }

    public async Task<Result<Updated>> ProcessOrderPaymentSucceededAsync(
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);

        if (order is null || order.Status != OrderStatus.Paid)
        {
            return Error.NotFound("Order.NotFoundOrNotPaid", "Order not found or payment is not completed.");
        }

        foreach (var item in order.Items)
        {
            await EnsureEnrollmentForCourseAccessAsync(order.StudentId, item.CourseId, cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Updated;
    }
}
