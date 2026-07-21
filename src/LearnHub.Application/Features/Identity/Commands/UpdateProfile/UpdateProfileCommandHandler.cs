using LearnHub.Application.common.Errors;
using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LearnHub.Application.Features.Identity.Commands.UpdateProfile;


public class UpdateProfileCommandHandler(IAppDbContext context,
ILogger<UpdateProfileCommandHandler> logger) : IRequestHandler<UpdateProfileCommand, Result<Updated>>

{
    private readonly IAppDbContext _context = context;
    private readonly ILogger<UpdateProfileCommandHandler> _logger = logger;

    public async Task<Result<Updated>> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);
        if (user is null)
        {
            return ApplicationErrors.UserNotFound;
        }

        var result = user.UpdateProfile(request.FirstName, request.LastName, request.PhoneNumber, request.DateOfBirth, request.Bio, request.Country);
        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to update profile for user {UserId}. Errors: {Errors}", request.Id, result.Errors);
            return result.Errors;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Updated;

    }
}