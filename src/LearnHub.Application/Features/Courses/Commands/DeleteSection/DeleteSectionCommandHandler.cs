using LearnHub.Application.common.Errors;
using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Courses.Commands.DeleteSection;

public sealed class DeleteSectionCommandHandler(IAppDbContext context) : IRequestHandler<DeleteSectionCommand, Result<Deleted>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<Deleted>> Handle(DeleteSectionCommand request, CancellationToken cancellationToken)
    {
        var section = await _context.Sections.FirstOrDefaultAsync(x => x.Id == request.SectionId, cancellationToken);
        if (section is null)
        {
            return Error.NotFound("ApplicationError.Course.SectionNotFound", "Section not found.");
        }

        _context.Sections.Remove(section);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Deleted;
    }
}