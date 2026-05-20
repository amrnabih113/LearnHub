using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Identity.Commands.RefreshToken;

public sealed record RefreshTokenCommand(string RefreshToken, string ExpiredToken) 
: IRequest<Result<TokenResponse>>;