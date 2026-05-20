using LearnHub.Application.common.Errors;
using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Common.Interfaces.Authentication;
using LearnHub.Application.Common.Interfaces.Notifications;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Identity;
using LearnHub.Domain.Identity.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Identity.Commands.Register;

public class RegisterCommandHandler(IAppDbContext context,
                                    IPasswordHasher passwordHasher,
                                    ITokenProvider tokenProvider) : IRequestHandler<RegisterCommand, Result<TokenResponse>>
{
    private readonly IAppDbContext _context = context;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly ITokenProvider _tokenProvider = tokenProvider;

    public async Task<Result<TokenResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (await _context.Users.AnyAsync(x => x.Email == request.Email, cancellationToken))
        {
            return ApplicationErrors.EmailAlreadyExists;
        }
        if (request.Password != request.ConfirmPassword)
        {
            return ApplicationErrors.PasswordsDontMatch;
        }

        var passwordHash = _passwordHasher.HashPassword(request.Password);
        if (passwordHash.IsError)
        {
            return passwordHash.Errors;
        }
        var user = User.Create(
            id: Guid.NewGuid(),
            firstName: request.FirstName,
            lastName: request.LastName,
            email: request.Email,
            passwordHash: passwordHash.Value,
            role: request.Role,
            phoneNumber: request.PhoneNumber ?? string.Empty);
        if (user.IsError)
        {
            return user.Errors;

        }
        await _context.Users.AddAsync(user.Value, cancellationToken);

        user.Value.AddDomainEvent(new UserCreatedDomainEvent
        {
            UserId = user.Value.Id,
            Email = user.Value.Email
        });
        await _context.SaveChangesAsync(cancellationToken);
        var tokenResult = await _tokenProvider.GenerateJwtTokenAsync(user.Value, cancellationToken);

        if (tokenResult.IsError)
        {
            return tokenResult.Errors;
        }
        return tokenResult.Value;

    }
}