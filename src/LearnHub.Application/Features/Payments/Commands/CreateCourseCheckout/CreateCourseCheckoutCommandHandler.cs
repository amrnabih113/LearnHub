using LearnHub.Application.common.Errors;
using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Payments.Dtos;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Purchasing.Enums;
using LearnHub.Domain.Purchasing.Orders;
using LearnHub.Domain.Purchasing.Payments;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Payments.Commands.CreateCourseCheckout;

public sealed class CreateCourseCheckoutCommandHandler(
    IAppDbContext context,
    IPaymentGatewayService paymentGatewayService,
    ICourseAccessService courseAccessService)
    : IRequestHandler<CreateCourseCheckoutCommand, Result<CheckoutSessionDto>>
{
    private readonly IAppDbContext _context = context;
    private readonly IPaymentGatewayService _paymentGatewayService = paymentGatewayService;
    private readonly ICourseAccessService _courseAccessService = courseAccessService;

    public async Task<Result<CheckoutSessionDto>> Handle(
        CreateCourseCheckoutCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == request.StudentId, cancellationToken);
        if (user is null)
        {
            return ApplicationErrors.UserNotFound;
        }

        var course = await _context.Courses
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.CourseId, cancellationToken);
        if (course is null)
        {
            return Error.NotFound("Course.NotFound", "Course not found.");
        }

        if (course.Price.Amount == 0)
        {
            await _courseAccessService.EnsureEnrollmentForCourseAccessAsync(user.Id, course.Id, cancellationToken);
            return new CheckoutSessionDto(
                SessionId: "free_course_enrolled",
                CheckoutUrl: request.SuccessUrl);
        }

        var alreadyPurchased = await _context.Orders
            .AsNoTracking()
            .AnyAsync(o => o.StudentId == request.StudentId
                        && o.Status == OrderStatus.Paid
                        && o.Items.Any(i => i.CourseId == request.CourseId), cancellationToken);
        if (alreadyPurchased)
        {
            return Error.Conflict("Course.AlreadyPurchased", "You have already purchased this course.");
        }

        var orderResult = Order.Create(Guid.NewGuid(), request.StudentId, course.Price.Currency);
        if (orderResult.IsError)
        {
            return orderResult.Errors;
        }

        var order = orderResult.Value;
        var addItemResult = order.AddItem(course.Id, course.Title ?? string.Empty, course.Price);

        if (addItemResult.IsError)
        {
            return addItemResult.Errors;
        }

        var checkoutResult = order.Checkout(DateTimeOffset.UtcNow);
        if (checkoutResult.IsError)
        {
            return checkoutResult.Errors;
        }

        var paymentResult = Payment.Create(Guid.NewGuid(), order.Id, PaymentProvider.Stripe, order.TotalAmount);
        if (paymentResult.IsError)
        {
            return paymentResult.Errors;
        }

        var payment = paymentResult.Value;
        _context.Orders.Add(order);
        _context.Payments.Add(payment);
        await _context.SaveChangesAsync(cancellationToken);

        var args = new CreateCheckoutSessionArgs(
            UserId: user.Id,
            UserEmail: user.Email,
            PaymentType: PaymentType.CoursePurchase,
            TargetId: course.Id,
            ItemTitle: course.Title ?? "",
            Amount: order.TotalAmount.Amount,
            Currency: order.TotalAmount.Currency,
            SuccessUrl: request.SuccessUrl,
            CancelUrl: request.CancelUrl,
            Metadata: new Dictionary<string, string>
            {
                ["orderId"] = order.Id.ToString(),
                ["paymentId"] = payment.Id.ToString(),
                ["studentId"] = user.Id.ToString(),
                ["courseId"] = course.Id.ToString()
            });

        var sessionResult = await _paymentGatewayService.CreateCheckoutSessionAsync(args, cancellationToken);
        if (sessionResult.IsError)
        {
            return sessionResult.Errors;
        }

        return new CheckoutSessionDto(sessionResult.Value.SessionId, sessionResult.Value.CheckoutUrl);
    }
}
