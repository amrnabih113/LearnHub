using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Admin.Dtos;
using LearnHub.Domain.Classification;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Admin.Queries.GetTagByIdAdmin;

public sealed class GetTagByIdAdminQueryHandler(IAppDbContext context)
    : IRequestHandler<GetTagByIdAdminQuery, Result<TagAdminDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<TagAdminDto>> Handle(
        GetTagByIdAdminQuery request,
        CancellationToken cancellationToken)
    {
        var tag = await _context.Tags
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (tag is null)
        {
            return TagErrors.TagNotFound;
        }

        return new TagAdminDto(
            tag.Id,
            tag.Name,
            tag.Slug,
            tag.Description,
            tag.CreatedAtUtc);
    }
}
