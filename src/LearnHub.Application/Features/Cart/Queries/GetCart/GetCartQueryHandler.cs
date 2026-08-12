using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Cart.Dtos;
using LearnHub.Application.Features.Cart.Services;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Purchasing.Carts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Cart.Queries.GetCart;

public sealed class GetCartQueryHandler(IAppDbContext context)
    : IRequestHandler<GetCartQuery, Result<CartDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<CartDto>> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.StudentId == request.StudentId, cancellationToken);

        if (cart is null)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.StudentId, cancellationToken);
            if (user is null)
            {
                return Error.NotFound("Cart.UserNotFound", "User not found.");
            }

            var currency = "USD";
            var createResult = Domain.Purchasing.Carts.Cart.Create(Guid.NewGuid(), request.StudentId, currency);
            if (createResult.IsError)
            {
                return createResult.Errors;
            }

            cart = createResult.Value;
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync(cancellationToken);
        }

        return await CartCalculator.CalculateAsync(cart, _context, cancellationToken);
    }
}
