using LearnHub.Application.common.Errors;
using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Courses.Commands.UpdateSection;

public sealed class UpdateSectionCommandHandler(IAppDbContext context) : IRequestHandler<UpdateSectionCommand, Result<Updated>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<Updated>> Handle(UpdateSectionCommand request, CancellationToken cancellationToken)
    {
        var section = await _context.Sections.FirstOrDefaultAsync(x => x.Id == request.SectionId, cancellationToken);
        if (section is null)
        {
            return Error.NotFound("ApplicationError.Course.SectionNotFound", "Section not found.");
        }

        var result = section.Update(request.Title, request.Description, request.Order);
        if (result.IsError)
        {
            return result.Errors;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Updated;
    }
}