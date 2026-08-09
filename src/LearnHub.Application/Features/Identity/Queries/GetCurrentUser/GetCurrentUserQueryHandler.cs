namespace LearnHub.Application.Features.Identity.Queries.GetCurrentUser;

using LearnHub.Application.common.Errors;
using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Common.Interfaces.Authentication;
using LearnHub.Application.Features.Identity.Dtos;
using LearnHub.Application.Features.Identity.Mappers;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class GetCurrentUserQueryHandler(IAppDbContext context,
 ICurrentUserService currentUserService) : IRequestHandler<GetCurrentUserQuery, Result<UserDto>>
{
    private readonly IAppDbContext _context = context;
    private readonly ICurrentUserService _currentUserService = currentUserService;

    public async Task<Result<UserDto>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated ||
        _currentUserService.UserId is null)
        {
            return Error.Unauthorized("User is not authenticated.");
        }
        var userId = _currentUserService.UserId;

        var user = await _context.Users.AsNoTracking()
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

        if (user is null)
        {
            return ApplicationErrors.UserNotFound;
        }

        var userDto = user.ToDto();

        if (userDto.IsError)
        {
            return userDto.Errors;
        }

        return userDto;
    }
}