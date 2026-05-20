using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Identity.Commands.Login;



public sealed record LoginCommand(
    string Email,
    string Password) : IRequest<Result<TokenResponse>>;

