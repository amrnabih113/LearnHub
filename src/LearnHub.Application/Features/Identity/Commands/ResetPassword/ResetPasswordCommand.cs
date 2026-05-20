using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Identity.Commands.ResetPassword;

public sealed record ResetPasswordCommand(
    string Email,
    string Otp,
    string NewPassword) : IRequest<Result<Updated>>;