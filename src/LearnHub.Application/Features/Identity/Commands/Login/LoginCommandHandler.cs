using LearnHub.Application.common.Errors;
using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Identity.Commands.ResendVerificationEmail;
using LearnHub.Domain.Identity;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RefreshTokenEntity = LearnHub.Domain.Identity.RefreshToken;

namespace LearnHub.Application.Features.Identity.Commands.Login;

public class LoginCommandHandler(
    ITokenProvider tokenProvider,
    IAppDbContext context,
    IPasswordHasher passwordHasher,
    ISender sender) : IRequestHandler<LoginCommand, Result<TokenResponse>>
{
    private readonly ITokenProvider _tokenProvider = tokenProvider;
    private readonly IAppDbContext _context = context;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly ISender _sender = sender;

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

            await _sender.Send(
                new SendVerificationEmailCommand(user.Email),
                cancellationToken);

            return ApplicationErrors.EmailNotVerified;
        }
        var tokenResult = await _tokenProvider.GenerateJwtTokenAsync(
            user,
            cancellationToken);

        if (tokenResult.IsError)
        {
            return tokenResult.Errors;
        }

        var refreshTokenValue = _tokenProvider.GenerateRefreshToken();
        var refreshTokenExpiresOnUtc = _tokenProvider.GetRefreshTokenExpiresOnUtc();

        var refreshTokenResult = RefreshTokenEntity.Create(
            Guid.NewGuid(),
            refreshTokenValue,
            user.Id,
            refreshTokenExpiresOnUtc);

        if (refreshTokenResult.IsError)
        {
            return refreshTokenResult.Errors;
        }

        tokenResult.Value.RefreshToken = refreshTokenValue;
        tokenResult.Value.RefreshTokenExpiresOnUtc = refreshTokenExpiresOnUtc;

        await _context.RefreshTokens.AddAsync(
            refreshTokenResult.Value,
            cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return tokenResult;
    }


}