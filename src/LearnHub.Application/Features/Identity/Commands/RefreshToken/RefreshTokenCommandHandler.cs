using LearnHub.Application.common.Errors;
using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using LearnHub.Domain.Identity;
using RefreshTokenEntity = LearnHub.Domain.Identity.RefreshToken;

namespace LearnHub.Application.Features.Identity.Commands.RefreshToken;

public class RefreshTokenCommandHandler(
    IAppDbContext context,
    ITokenProvider tokenProvider)
    : IRequestHandler<RefreshTokenCommand, Result<TokenResponse>>
{
    private readonly IAppDbContext _context = context;
    private readonly ITokenProvider _tokenProvider = tokenProvider;

    public async Task<Result<TokenResponse>> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        var principal = _tokenProvider
            .GetPrincipalFromExpiredToken(request.ExpiredToken);

        if (principal is null)
        {
            return ApplicationErrors.InvalidRefreshToken;
        }

        var email = principal.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

        if (string.IsNullOrWhiteSpace(email))
        {
            return ApplicationErrors.InvalidRefreshToken;
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(
                x => x.Email == email,
                cancellationToken);

        if (user is null)
        {
            return ApplicationErrors.InvalidRefreshToken;
        }

        var refreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(
                x => x.Token == request.RefreshToken &&
                     x.UserId == user.Id,
                cancellationToken);

        if (refreshToken is null)
        {
            return ApplicationErrors.InvalidRefreshToken;
        }

        if (refreshToken.IsRevoked)
        {
            return ApplicationErrors.InvalidRefreshToken;
        }

        if (refreshToken.ExpiresOnUtc <= DateTimeOffset.UtcNow)
        {
            return ApplicationErrors.RefreshTokenExpired;
        }

        refreshToken.Revoke();

        var tokenResult = await _tokenProvider
            .GenerateJwtTokenAsync(user, cancellationToken);

        if (tokenResult.IsError)
        {
            return tokenResult.Errors;
        }

        var newRefreshTokenValue = _tokenProvider.GenerateRefreshToken();
        var newRefreshTokenExpiresOnUtc = _tokenProvider.GetRefreshTokenExpiresOnUtc();

        var newRefreshTokenResult = RefreshTokenEntity.Create(
            Guid.NewGuid(),
            newRefreshTokenValue,
            user.Id,
            newRefreshTokenExpiresOnUtc);

        if (newRefreshTokenResult.IsError)
        {
            return newRefreshTokenResult.Errors;
        }

        tokenResult.Value.RefreshToken = newRefreshTokenValue;
        tokenResult.Value.RefreshTokenExpiresOnUtc = newRefreshTokenExpiresOnUtc;

        await _context.RefreshTokens.AddAsync(
            newRefreshTokenResult.Value,
            cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return tokenResult.Value;
    }
}