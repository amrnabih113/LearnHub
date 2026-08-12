using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Cart.Dtos;
using LearnHub.Application.Features.Cart.Services;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Purchasing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Cart.Commands.RemoveCouponFromCart;

public sealed class RemoveCouponFromCartCommandHandler(IAppDbContext context)
    : IRequestHandler<RemoveCouponFromCartCommand, Result<CartDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<CartDto>> Handle(RemoveCouponFromCartCommand request, CancellationToken cancellationToken)
    {
        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.StudentId == request.StudentId, cancellationToken);

        if (cart is null)
        {
            return CartErrors.ItemNotFound;
        }

        cart.RemoveCoupon();
        await _context.SaveChangesAsync(cancellationToken);

        return await CartCalculator.CalculateAsync(cart, _context, cancellationToken);
    }
}
