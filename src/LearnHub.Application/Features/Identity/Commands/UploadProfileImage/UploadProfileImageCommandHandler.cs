using LearnHub.Application.common.Errors;
using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Common.Interfaces.Authentication;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Identity.Commands.UploadProfileImage;


public class UploadProfileImageCommandHandler(ICurrentUserService currentUserService,
 IFileStorageService fileStorageService,
 IAppDbContext Context) : IRequestHandler<UploadProfileImageCommand, Result<string>>
{
    private readonly ICurrentUserService _currentUserService = currentUserService;
    private readonly IFileStorageService _fileStorageService = fileStorageService;

    private readonly IAppDbContext _context = Context;
    public async Task<Result<string>> Handle(UploadProfileImageCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated || _currentUserService.UserId is null)
        {
            return Error.Unauthorized("User is not authenticated.");
        }

        var userId = _currentUserService.UserId;

        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (user is null)
        {
            return ApplicationErrors.UserNotFound;
        }

        var oldImageUrl = user.ImageUrl;

        var uploadResult = await _fileStorageService.UploadImageAsync(
            request.Image,
            "profile-images",
            cancellationToken);

        if (uploadResult.IsError)
        {
            return uploadResult.Errors;
        }

        var imageUrl = uploadResult.Value;

        var updateResult = user.UpdateProfileImage(imageUrl);

        if (updateResult.IsError)
        {
            return updateResult.Errors;
        }
        await _context.SaveChangesAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(oldImageUrl))
        {
            await _fileStorageService.DeleteImageAsync(
                oldImageUrl,
                cancellationToken);
        }

        return imageUrl;
    }
}