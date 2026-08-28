using LearnHub.Application.common.Errors;
using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Common.Interfaces.Authentication;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Identity;
using LearnHub.Domain.Instructor;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Identity.Commands.RegisterInstructor;

public sealed record RegisterInstructorCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string ConfirmPassword,
    string? ProfessionalTitle = null,
    string? Headline = null,
    string? Biography = null,
    string? PhoneNumber = null) : IRequest<Result<Created>>;

public sealed class RegisterInstructorCommandHandler(
    IAppDbContext context,
    IPasswordHasher passwordHasher)
    : IRequestHandler<RegisterInstructorCommand, Result<Created>>
{
    private readonly IAppDbContext _context = context;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;

    public async Task<Result<Created>> Handle(RegisterInstructorCommand request, CancellationToken cancellationToken)
    {
        if (await _context.Users.AnyAsync(x => x.Email == request.Email, cancellationToken))
        {
            return ApplicationErrors.EmailAlreadyExists;
        }
        if (request.Password != request.ConfirmPassword)
        {
            return ApplicationErrors.PasswordsDontMatch;
        }

        var passwordHash = _passwordHasher.HashPassword(request.Password);
        if (passwordHash.IsError)
        {
            return passwordHash.Errors;
        }

        var userResult = User.Create(
            id: Guid.NewGuid(),
            firstName: request.FirstName,
            lastName: request.LastName,
            email: request.Email,
            passwordHash: passwordHash.Value,
            role: Role.Instructor, // Hardcoded role boundary
            phoneNumber: request.PhoneNumber ?? string.Empty);

        if (userResult.IsError)
        {
            return userResult.Errors;
        }

        var user = userResult.Value;
        await _context.Users.AddAsync(user, cancellationToken);

        // Initialize InstructorProfile in Pending status
        var profileResult = InstructorProfile.Create(
            user.Id,
            request.ProfessionalTitle,
            request.Headline,
            request.Biography);

        if (profileResult.IsError)
        {
            return profileResult.Errors;
        }

        _context.InstructorProfiles.Add(profileResult.Value);

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Created;
    }
}
