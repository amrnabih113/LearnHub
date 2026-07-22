using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Common.Interfaces.Authentication;
using LearnHub.Application.Common.Interfaces.Notifications;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Common.Results.Abstractions;
using LearnHub.Domain.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Identity.Commands.ForgotPassword;

public class ForgetPasswordCommandHandler(IAppDbContext context, IOtpProvider otpProvider, IEmailService emailService
) : IRequestHandler<ForgetPasswordCommand, Result<Created>>
{
    private readonly IAppDbContext _context = context;
    private readonly IOtpProvider _otpProvider = otpProvider;
    private readonly IEmailService _emailService = emailService;

    public async Task<Result<Created>> Handle(ForgetPasswordCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email;
        var user = await _context.Users
            .FirstOrDefaultAsync(
                x => x.Email == email,
                cancellationToken);

        if (user is null)
        {
            return Result.Created;
        }

        var otpResult = _otpProvider.GenerateOtp(OtpPurpose.PasswordReset);

        var otpHash = _otpProvider.HashOtp(otpResult);
        var otpCode = OtpCode.Create(
           id: Guid.NewGuid(),
           userId: user.Id,
           codeHash: otpHash,
           purpose: OtpPurpose.PasswordReset,
           expiresAtUtc: DateTimeOffset.UtcNow.AddMinutes(10));

        if (otpCode.IsError)
        {
            return otpCode.Errors;
        }

        await _context.OtpCodes.AddAsync(otpCode.Value, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        await _emailService.SendAsync(
            to: email,
            subject: "Password Reset OTP",
            body: $"Your OTP code for password reset is: {otpResult}");

        return Result.Created;
    }
}