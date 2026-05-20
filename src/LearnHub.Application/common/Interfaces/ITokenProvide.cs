using System.Security.Claims;
using LearnHub.Application.Features.Identity;
using LearnHub.Application.Features.Identity.Dtos;
using LearnHub.Domain.Common.Results;

namespace LearnHub.Application.common.Interfaces;

public interface ITokenProvider
{
    Task<Result<TokenResponse>> GenerateJwtTokenAsync(UserDto user, CancellationToken ct = default);

    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}