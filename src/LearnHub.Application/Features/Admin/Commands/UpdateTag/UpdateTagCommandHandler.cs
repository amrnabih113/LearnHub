using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Admin.Dtos;
using LearnHub.Domain.Classification;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Admin.Commands.UpdateTag;

public sealed class UpdateTagCommandHandler(IAppDbContext context)
    : IRequestHandler<UpdateTagCommand, Result<TagAdminDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<TagAdminDto>> Handle(
        UpdateTagCommand request,
        CancellationToken cancellationToken)
    {
        var tag = await _context.Tags
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (tag is null)
        {
            return TagErrors.TagNotFound;
        }

        var duplicateExists = await _context.Tags
            .AsNoTracking()
            .AnyAsync(t => t.Id != request.Id && (t.Name == request.Name.Trim() || t.Slug == request.Slug.Trim().ToLowerInvariant()), cancellationToken);

        if (duplicateExists)
        {
            return TagErrors.DuplicateName;
        }

        var renameResult = tag.Rename(request.Name, request.Slug, request.Description);
        if (renameResult.IsError)
        {
            return renameResult.Errors;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return new TagAdminDto(
            tag.Id,
            tag.Name,
            tag.Slug,
            tag.Description,
            tag.CreatedAtUtc);
    }
}
