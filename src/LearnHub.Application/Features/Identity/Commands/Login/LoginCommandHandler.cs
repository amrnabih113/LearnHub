using LearnHub.Application.common.Errors;
using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Identity.Commands.Login;

public class LoginCommandHandler(ITokenProvider tokenProvider,
                                 IAppDbContext context,
                                 IPasswordHasher passwordHasher) : IRequestHandler<LoginCommand, Result<TokenResponse>>
{
    private readonly ITokenProvider _tokenProvider = tokenProvider;
    private readonly IAppDbContext _context = context;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;

    public async Task<Result<TokenResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == request.Email, cancellationToken);
        if (user is null)
        {
            return ApplicationErrors.InvalidCredentials;
        }
        var isPasswordValid = _passwordHasher.VerifyPassword(
            request.Password,
            user.PasswordHash);

        if (!isPasswordValid)
        {
            return ApplicationErrors.InvalidCredentials;
        }

        if (!user.IsEmailVerified)
        { 
            return ApplicationErrors.EmailNotVerified;
        }
        var tokenResult = await _tokenProvider.GenerateJwtTokenAsync(
            user,
            cancellationToken);

        if (tokenResult.IsError)
        {
            return tokenResult.Errors;
        }

        return tokenResult;
    }

    
}