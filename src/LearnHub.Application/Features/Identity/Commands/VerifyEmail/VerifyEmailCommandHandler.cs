using LearnHub.Application.common.Errors;
using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Identity.Commands.VerifyEmail;


public class VerifyEmailCommandHandler(IAppDbContext context) : IRequestHandler<VerifyEmailCommand, Result<Updated>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<Updated>> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == request.Email, cancellationToken);
        if (user == null)
        {
            return ApplicationErrors.UserNotFound;
        }
        user.VerifyEmail();
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Updated;
    }
}
