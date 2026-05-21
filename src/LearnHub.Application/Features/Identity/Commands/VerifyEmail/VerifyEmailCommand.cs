using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Identity.Commands.VerifyEmail;


public sealed record VerifyEmailCommand(
    string Email,
    string Otp) : IRequest<Result<Updated>>;