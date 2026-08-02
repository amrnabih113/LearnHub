using LearnHub.Application.common.Errors;
using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Common.Interfaces.Authentication;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Identity.Commands.VerifyForgotPasswordOtp;

public sealed class VerifyForgotPasswordOtpCommandHandler(
    IAppDbContext context,
    IOtpProvider otpProvider) : IRequestHandler<VerifyForgotPasswordOtpCommand, Result<PasswordResetTokenResponse>>
{
    private readonly IAppDbContext _context = context;
    private readonly IOtpProvider _otpProvider = otpProvider;

    public async Task<Result<PasswordResetTokenResponse>> Handle(
        VerifyForgotPasswordOtpCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Email == request.Email, cancellationToken);

        if (user is null)
        {
            return ApplicationErrors.UserNotFound;
        }

        var hashedOtp = _otpProvider.HashOtp(request.Otp);

        var otpCode = await _context.OtpCodes
            .FirstOrDefaultAsync(x =>
                x.UserId == user.Id &&
                x.Purpose == OtpPurpose.PasswordReset &&
                x.CodeHash == hashedOtp,
                cancellationToken);

        if (otpCode is null)
        {
            return ApplicationErrors.InvalidOtp;
        }

        if (otpCode.ExpiresAtUtc <= DateTimeOffset.UtcNow)
        {
            return ApplicationErrors.OtpExpired;
        }

        if (otpCode.UsedAtUtc.HasValue)
        {
            return ApplicationErrors.InvalidOtp;
        }

        var resetTokenValue = Convert.ToHexString(System.Security.Cryptography.RandomNumberGenerator.GetBytes(32));
        var expiresOnUtc = DateTimeOffset.UtcNow.AddMinutes(15);

        var resetTokenResult = PasswordResetToken.Create(
            Guid.NewGuid(),
            resetTokenValue,
            user.Id,
            expiresOnUtc);

        if (resetTokenResult.IsError)
        {
            return resetTokenResult.Errors;
        }

        var oldResetTokens = _context.PasswordResetTokens.Where(x =>
            x.UserId == user.Id &&
            !x.UsedAtUtc.HasValue);

        _context.PasswordResetTokens.RemoveRange(oldResetTokens);

        otpCode.MarkUsed();

        await _context.PasswordResetTokens.AddAsync(resetTokenResult.Value, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return new PasswordResetTokenResponse(
            resetTokenValue,
            expiresOnUtc);
    }
}