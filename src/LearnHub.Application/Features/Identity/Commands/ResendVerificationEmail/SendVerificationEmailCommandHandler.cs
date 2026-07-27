namespace LearnHub.Application.Features.Identity.Commands.ResendVerificationEmail;

using LearnHub.Application.common.Errors;
using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Common.Interfaces.Authentication;
using LearnHub.Application.Common.Interfaces;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using LearnHub.Application.Common.Models;

public class SendVerificationEmailCommandHandler(IBackgroundJobService backgroundJobService, ILogger<SendVerificationEmailCommandHandler> logger, IAppDbContext context, IOtpProvider otpProvider) : IRequestHandler<SendVerificationEmailCommand, Result<Created>>
{
    private readonly IBackgroundJobService _backgroundJobService = backgroundJobService;
    private readonly ILogger<SendVerificationEmailCommandHandler> _logger = logger;
    private readonly IAppDbContext _context = context;
    private readonly IOtpProvider _otpProvider = otpProvider;

    public async Task<Result<Created>> Handle(SendVerificationEmailCommand request, CancellationToken cancellationToken)
    {
        // check if the user exists and is not verified
        var email = request.Email;
        var user = await _context.Users
                   .FirstOrDefaultAsync(
                       x => x.Email == email,
                       cancellationToken);
        if (user is null)
        {
            return Result.Created;
        }
        if (user.IsEmailVerified)
        {
            return ApplicationErrors.EmailAlreadyVerified;
        }

        // Remove any existing OTP codes for email verification for this user
        var oldOtps = _context.OtpCodes.Where(x =>
        x.UserId == user.Id &&
        x.Purpose == OtpPurpose.EmailVerification);

        _context.OtpCodes.RemoveRange(oldOtps);

        // Generate a new OTP code for email verification
        var otpResult = _otpProvider.GenerateOtp();

        var otpHash = _otpProvider.HashOtp(otpResult);
        var otpCode = OtpCode.Create(
           id: Guid.NewGuid(),
           userId: user.Id,
           codeHash: otpHash,
           purpose: OtpPurpose.EmailVerification,
           expiresAtUtc: DateTimeOffset.UtcNow.AddMinutes(10));

        if (otpCode.IsError)
        {
            return otpCode.Errors;
        }

        await _context.OtpCodes.AddAsync(otpCode.Value, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        // Send the OTP code to the user's email
        _backgroundJobService.QueueEmail(
      new EmailMessage(
          email,
          "Reset Your Password",
          EmailTemplate.PasswordReset,
          new Dictionary<string, string>
          {
              ["Name"] = user.FirstName,
              ["Otp"] = otpResult
          }));

        return Result.Created;
    }
}