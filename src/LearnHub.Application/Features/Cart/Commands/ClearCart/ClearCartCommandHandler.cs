using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Cart.Dtos;
using LearnHub.Application.Features.Cart.Services;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Cart.Commands.ClearCart;

public sealed class ClearCartCommandHandler(IAppDbContext context)
    : IRequestHandler<ClearCartCommand, Result<CartDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<CartDto>> Handle(ClearCartCommand request, CancellationToken cancellationToken)
    {
        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.StudentId == request.StudentId, cancellationToken);

        if (cart is not null)
        {
            cart.Clear();
            await _context.SaveChangesAsync(cancellationToken);
        }
        else
        {
            var currency = "USD";
            var createCartResult = Domain.Purchasing.Carts.Cart.Create(Guid.NewGuid(), request.StudentId, currency);
            if (createCartResult.IsSuccess)
            {
                cart = createCartResult.Value;
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        return await CartCalculator.CalculateAsync(cart!, _context, cancellationToken);
    }
}
