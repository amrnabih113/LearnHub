using LearnHub.Application.Features.Identity;
using LearnHub.Application.Features.Identity.Dtos;
using LearnHub.Domain.Common.Results;

namespace LearnHub.Application.Common.Interfaces.Authentication;

public interface IAuthenticationService
{
    Task<Result<TokenResponse>> LoginAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default);
}