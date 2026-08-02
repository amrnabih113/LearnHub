using LearnHub.Application.common.Errors;
using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Common.Interfaces.Authentication;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Identity.Commands.ResetPassword;

public class ResetPasswordCommandHandler(IAppDbContext context,
                                         IPasswordHasher passwordHasher
                                        ) : IRequestHandler<ResetPasswordCommand, Result<Updated>>
{
    private readonly IAppDbContext _context = context;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;

    public async Task<Result<Updated>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var resetToken = await _context.PasswordResetTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Token == request.ResetToken, cancellationToken);

        if (resetToken is null)
        {
            return ApplicationErrors.InvalidResetToken;
        }

        var hashedNewPassword = _passwordHasher.HashPassword(request.NewPassword);

        if (hashedNewPassword.IsError)
        {
            return hashedNewPassword.Errors;
        }

        if (resetToken.IsUsed)
        {
            return ApplicationErrors.InvalidResetToken;
        }

        if (resetToken.ExpiresOnUtc <= DateTimeOffset.UtcNow)
        {
            return ApplicationErrors.ResetTokenExpired;
        }

        resetToken.User.ChangePassword(hashedNewPassword.Value);

        resetToken.Consume();

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Updated;
    }
}