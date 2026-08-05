using LearnHub.Application.common.Errors;
using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Courses.Commands.DeleteResource;

public sealed class DeleteResourceCommandHandler(IAppDbContext context) : IRequestHandler<DeleteResourceCommand, Result<Deleted>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<Deleted>> Handle(DeleteResourceCommand request, CancellationToken cancellationToken)
    {
        var resource = await _context.Resources.FirstOrDefaultAsync(x => x.Id == request.ResourceId, cancellationToken);
        if (resource is null)
        {
            return Error.NotFound("ApplicationError.Course.ResourceNotFound", "Resource not found.");
        }

        _context.Resources.Remove(resource);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Deleted;
    }
}