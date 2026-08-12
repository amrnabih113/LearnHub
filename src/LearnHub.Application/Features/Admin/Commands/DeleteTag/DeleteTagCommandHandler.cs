using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Classification;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Admin.Commands.DeleteTag;

public sealed class DeleteTagCommandHandler(IAppDbContext context)
    : IRequestHandler<DeleteTagCommand, Result<Deleted>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<Deleted>> Handle(
        DeleteTagCommand request,
        CancellationToken cancellationToken)
    {
        var tag = await _context.Tags
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (tag is null)
        {
            return TagErrors.TagNotFound;
        }

        _context.Tags.Remove(tag);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Deleted;
    }
}
