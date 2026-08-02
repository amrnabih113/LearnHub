using LearnHub.Application.Features.Identity;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Identity.Commands.VerifyForgotPasswordOtp;

public sealed record VerifyForgotPasswordOtpCommand(
    string Email,
    string Otp) : IRequest<Result<PasswordResetTokenResponse>>;