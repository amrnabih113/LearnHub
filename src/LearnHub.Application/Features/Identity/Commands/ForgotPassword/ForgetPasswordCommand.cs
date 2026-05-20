using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Common.Results.Abstractions;
using MediatR;

namespace LearnHub.Application.Features.Identity.Commands.ForgotPassword;

public sealed record ForgetPasswordCommand(string Email) : IRequest<Result<Created>>;