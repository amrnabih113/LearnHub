using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Student.Dtos;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Student.Commands.UpdateStudentProfile;

public sealed class UpdateStudentProfileCommandHandler(IAppDbContext context)
    : IRequestHandler<UpdateStudentProfileCommand, Result<StudentProfileDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<StudentProfileDto>> Handle(
        UpdateStudentProfileCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == request.StudentId, cancellationToken);

        if (user is null)
        {
            return Error.NotFound("User.NotFound", "User was not found.");
        }

        var updateResult = user.UpdateProfile(
            request.FirstName,
            request.LastName,
            request.PhoneNumber,
            request.DateOfBirth,
            request.Bio,
            request.Country);

        if (updateResult.IsError)
        {
            return updateResult.Errors;
        }

        await _context.SaveChangesAsync(cancellationToken);

        var roles = user.Roles.Select(r => r.Role.ToString()).ToList();

        return new StudentProfileDto(
            user.Id,
            user.FirstName,
            user.LastName,
            $"{user.FirstName} {user.LastName}",
            user.Email,
            user.PhoneNumber,
            user.ImageUrl,
            user.DateOfBirth,
            user.Bio,
            user.Country,
            user.IsEmailVerified,
            roles,
            user.CreatedAtUtc);
    }
}
