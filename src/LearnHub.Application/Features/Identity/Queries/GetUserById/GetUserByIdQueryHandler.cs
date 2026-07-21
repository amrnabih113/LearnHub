namespace LearnHub.Application.Features.Identity.Queries.GetUserById;

using LearnHub.Application.common.Errors;
using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Identity.Dtos;
using LearnHub.Application.Features.Identity.Mappers;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;


public class GetUserByIdQueryHandler(
    IAppDbContext context)
    : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<UserDto>> Handle(
        GetUserByIdQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == request.Id,
                cancellationToken);

        if (user is null)
        {
            return ApplicationErrors.UserNotFound;
        }

        return user.ToDto();
    }
}