using System.Security.Claims;
using LearnHub.Application.Features.Identity;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Identity;

namespace LearnHub.Application.common.Interfaces;

public interface ITokenProvider
{
    Task<Result<TokenResponse>> GenerateJwtTokenAsync(User user, CancellationToken ct = default);

    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}