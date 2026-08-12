using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Cart.Dtos;
using LearnHub.Application.Features.Cart.Services;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Enrollments.Enums;
using LearnHub.Domain.Purchasing;
using LearnHub.Domain.Purchasing.Carts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Cart.Commands.AddToCart;

public sealed class AddToCartCommandHandler(IAppDbContext context)
    : IRequestHandler<AddToCartCommand, Result<CartDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<CartDto>> Handle(AddToCartCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == request.StudentId, cancellationToken);
        if (user is null)
        {
            return Error.NotFound("User.NotFound", "User not found.");
        }

        var course = await _context.Courses
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.CourseId, cancellationToken);
        if (course is null)
        {
            return Error.NotFound("Course.NotFound", "Course not found.");
        }

        var existingEnrollment = await _context.Enrollments
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.StudentId == request.StudentId && e.CourseId == request.CourseId, cancellationToken);

        if (existingEnrollment is not null && existingEnrollment.Status is EnrollmentStatus.Active or EnrollmentStatus.Completed)
        {
            return CartErrors.CourseAlreadyEnrolled;
        }

        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.StudentId == request.StudentId, cancellationToken);

        if (cart is null)
        {
            var currency = course.Price.Currency;
            var createCartResult = Domain.Purchasing.Carts.Cart.Create(Guid.NewGuid(), request.StudentId, currency);
            if (createCartResult.IsError)
            {
                return createCartResult.Errors;
            }

            cart = createCartResult.Value;
            _context.Carts.Add(cart);
        }

        var addItemResult = cart.AddItem(course.Id, course.Title, course.Price);
        if (addItemResult.IsError)
        {
            return addItemResult.Errors;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return await CartCalculator.CalculateAsync(cart, _context, cancellationToken);
    }
}
