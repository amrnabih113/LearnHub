using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Identity.Commands.ResetPassword;

public sealed record ResetPasswordCommand(
    string ResetToken,
    string NewPassword) : IRequest<Result<Updated>>;