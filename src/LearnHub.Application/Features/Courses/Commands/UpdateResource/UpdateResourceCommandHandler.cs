using LearnHub.Application.common.Errors;
using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Courses.Commands.UpdateResource;

public sealed class UpdateResourceCommandHandler(IAppDbContext context) : IRequestHandler<UpdateResourceCommand, Result<Updated>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<Updated>> Handle(UpdateResourceCommand request, CancellationToken cancellationToken)
    {
        var resource = await _context.Resources.FirstOrDefaultAsync(x => x.Id == request.ResourceId, cancellationToken);
        if (resource is null)
        {
            return Error.NotFound("ApplicationError.Course.ResourceNotFound", "Resource not found.");
        }

        var result = resource.Update(request.Name, request.Url, request.Type, request.SizeInBytes);
        if (result.IsError)
        {
            return result.Errors;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Updated;
    }
}