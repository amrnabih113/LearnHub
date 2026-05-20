using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Identity;
using MediatR;

namespace LearnHub.Application.Features.Identity.Commands.Register;


public sealed record RegisterCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string ConfirmPassword,
    Role Role,
    string? PhoneNumber = null) : IRequest<Result<TokenResponse>>;