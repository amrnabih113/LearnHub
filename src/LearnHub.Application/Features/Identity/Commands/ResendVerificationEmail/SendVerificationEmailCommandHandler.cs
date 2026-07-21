namespace LearnHub.Application.Features.Identity.Commands.ResendVerificationEmail;

using LearnHub.Application.common.Errors;
using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Common.Interfaces.Authentication;
using LearnHub.Application.Common.Interfaces.Notifications;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Common.Results.Abstractions;
using LearnHub.Domain.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


public class SendVerificationEmailCommandHandler(IEmailService emailService, ILogger<SendVerificationEmailCommandHandler> logger, IAppDbContext context, IOtpProvider otpProvider) : IRequestHandler<SendVerificationEmailCommand, Result<Created>>
{
    private readonly IEmailService _emailService = emailService;
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
            return ApplicationErrors.UserNotFound;
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
        await _emailService.SendAsync(
            to: email,
            subject: "Email Verification OTP",
            body: $"Your OTP code for email verification is: {otpResult}");

        return Result.Created;
    }
}