using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Instructor.Commands.UpdateInstructorProfile;

public sealed record UpdateInstructorProfileCommand(
    Guid InstructorUserId,
    string? ProfessionalTitle,
    string? Headline,
    string? Biography) : IRequest<Result<Updated>>;

public sealed class UpdateInstructorProfileCommandHandler(IAppDbContext context)
    : IRequestHandler<UpdateInstructorProfileCommand, Result<Updated>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<Updated>> Handle(
        UpdateInstructorProfileCommand request,
        CancellationToken cancellationToken)
    {
        var profile = await _context.InstructorProfiles
            .FirstOrDefaultAsync(p => p.UserId == request.InstructorUserId, cancellationToken);

        if (profile is null)
        {
            return Error.NotFound("InstructorProfile.NotFound", "Instructor profile was not found.");
        }

        var updateResult = profile.UpdateBasicInfo(
            request.ProfessionalTitle,
            request.Headline,
            request.Biography);

        if (updateResult.IsError)
        {
            return updateResult.Errors;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Updated;
    }
}
