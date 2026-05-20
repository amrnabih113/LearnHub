using LearnHub.Application.common.Errors;
using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Common.Interfaces.Authentication;
using LearnHub.Application.Common.Interfaces.Notifications;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Identity.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Identity.Commands.ResetPassword;

public class ResetPasswordCommandHandler(IAppDbContext context,
                                         IOtpProvider otpProvider,
                                         IPasswordHasher passwordHasher
                                        ) : IRequestHandler<ResetPasswordCommand, Result<Updated>>
{
    private readonly IAppDbContext _context = context;
    private readonly IOtpProvider _otpProvider = otpProvider;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;

    public async Task<Result<Updated>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == request.Email, cancellationToken);
        if (user == null)
        {
            return ApplicationErrors.UserNotFound;
        }

        var otpCode = await _context.OtpCodes.FirstOrDefaultAsync(x => x.UserId == user.Id, cancellationToken);

        var hashedOtp = _otpProvider.HashOtp(request.Otp);

        var hashedNewPassword = _passwordHasher.HashPassword(request.NewPassword);


        if (hashedNewPassword.IsError)
        {
            return hashedNewPassword.Errors;
        }

        request = request with
        {
            Otp = hashedOtp,
            NewPassword = hashedNewPassword.Value
        };

        if (otpCode == null || otpCode.ExpiresAtUtc < DateTime.UtcNow || otpCode.CodeHash != request.Otp)
        {
            return ApplicationErrors.InvalidOtp;
        }

        user.ChangePassword(request.NewPassword);

        _context.OtpCodes.Remove(otpCode);
        await _context.SaveChangesAsync(cancellationToken);

        user.AddDomainEvent(new PasswordChangedDomainEvent(user.Id));

        return Result.Updated;
    }
}