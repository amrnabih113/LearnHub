using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Instructor;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Instructor.Commands.AddInstructorLink;

public sealed record AddInstructorLinkCommand(
    Guid InstructorUserId,
    string Title,
    string Url) : IRequest<Result<Guid>>;

public sealed class AddInstructorLinkCommandHandler(IAppDbContext context)
    : IRequestHandler<AddInstructorLinkCommand, Result<Guid>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<Guid>> Handle(
        AddInstructorLinkCommand request,
        CancellationToken cancellationToken)
    {
        var profile = await _context.InstructorProfiles
            .Include(p => p.Links)
            .FirstOrDefaultAsync(p => p.UserId == request.InstructorUserId, cancellationToken);

        if (profile is null)
        {
            return Error.NotFound("InstructorProfile.NotFound", "Instructor profile was not found.");
        }

        var linkResult = InstructorLink.Create(
            Guid.NewGuid(),
            profile.Id,
            request.Title,
            request.Url);

        if (linkResult.IsError)
        {
            return linkResult.Errors;
        }

        var link = linkResult.Value;
        profile.AddLink(link);

        await _context.SaveChangesAsync(cancellationToken);
        return link.Id;
    }
}
