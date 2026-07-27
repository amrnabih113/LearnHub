using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Common.Interfaces;
using LearnHub.Application.Common.Interfaces.Authentication;
using LearnHub.Application.Common.Models;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Identity.Commands.ForgotPassword;

public class ForgetPasswordCommandHandler(IAppDbContext context, IOtpProvider otpProvider, IEmailQueue _emailQueue
) : IRequestHandler<ForgetPasswordCommand, Result<Created>>
{
    private readonly IAppDbContext _context = context;
    private readonly IOtpProvider _otpProvider = otpProvider;
    private readonly IEmailQueue _emailQueue = _emailQueue;

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



        var otpResult = _otpProvider.GenerateOtp();
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

        var oldOtps =
        _context.OtpCodes.Where(x =>
        x.UserId == user.Id &&
        x.Purpose == OtpPurpose.PasswordReset);

        _context.OtpCodes.RemoveRange(oldOtps);

        await _context.SaveChangesAsync(cancellationToken);

        await _emailQueue.QueueAsync(
        new EmailMessage(
            email,
            "Reset Your Password",
            EmailTemplate.PasswordReset,
            new Dictionary<string, string>
            {
                ["Name"] = user.FirstName,
                ["Otp"] = otpResult
            }),
        cancellationToken);

        return Result.Created;
    }
}