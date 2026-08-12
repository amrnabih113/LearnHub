using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Admin.Dtos;
using LearnHub.Domain.Classification;
using LearnHub.Domain.Classification.Tags;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Admin.Commands.CreateTag;

public sealed class CreateTagCommandHandler(IAppDbContext context)
    : IRequestHandler<CreateTagCommand, Result<TagAdminDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<TagAdminDto>> Handle(
        CreateTagCommand request,
        CancellationToken cancellationToken)
    {
        var nameOrSlugExists = await _context.Tags
            .AsNoTracking()
            .AnyAsync(t => t.Name == request.Name.Trim() || t.Slug == request.Slug.Trim().ToLowerInvariant(), cancellationToken);

        if (nameOrSlugExists)
        {
            return TagErrors.DuplicateName;
        }

        var tagResult = Tag.Create(Guid.NewGuid(), request.Name, request.Slug, request.Description);
        if (tagResult.IsError)
        {
            return tagResult.Errors;
        }

        var tag = tagResult.Value;
        _context.Tags.Add(tag);
        await _context.SaveChangesAsync(cancellationToken);

        return new TagAdminDto(
            tag.Id,
            tag.Name,
            tag.Slug,
            tag.Description,
            tag.CreatedAtUtc);
    }
}
