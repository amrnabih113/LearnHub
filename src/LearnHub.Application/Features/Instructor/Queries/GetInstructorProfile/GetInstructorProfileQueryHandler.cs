using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Instructor.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Instructor.Queries.GetInstructorProfile;

public sealed record GetInstructorProfileQuery(Guid UserId)
    : IRequest<Result<InstructorProfileDto>>;

public sealed class GetInstructorProfileQueryHandler(IAppDbContext context)
    : IRequestHandler<GetInstructorProfileQuery, Result<InstructorProfileDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<InstructorProfileDto>> Handle(
        GetInstructorProfileQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user is null)
        {
            return Error.NotFound("User.NotFound", "User was not found.");
        }

        var profile = await _context.InstructorProfiles
            .AsNoTracking()
            .Include(p => p.Experiences)
            .Include(p => p.Education)
            .Include(p => p.Certifications)
            .Include(p => p.Skills)
            .Include(p => p.Languages)
            .Include(p => p.Links)
            .FirstOrDefaultAsync(p => p.UserId == request.UserId, cancellationToken);

        if (profile is null)
        {
            return Error.NotFound("InstructorProfile.NotFound", "Instructor profile was not found.");
        }

        var experiences = profile.Experiences
            .Select(e => new InstructorExperienceDto(e.Id, e.JobTitle, e.Company, e.Description, e.StartDate, e.EndDate, e.IsCurrent, e.Location))
            .ToList();

        var education = profile.Education
            .Select(e => new InstructorEducationDto(e.Id, e.Institution, e.Degree, e.FieldOfStudy, e.StartDate, e.EndDate, e.Description))
            .ToList();

        var certifications = profile.Certifications
            .Select(c => new InstructorCertificationDto(c.Id, c.Name, c.IssuingOrganization, c.IssueDate, c.ExpirationDate, c.CredentialId, c.CredentialUrl))
            .ToList();

        var skills = profile.Skills.Select(s => s.SkillName).ToList();
        var links = profile.Links.Select(l => new InstructorLinkDto(l.Id, l.Title, l.Url)).ToList();

        return new InstructorProfileDto(
            user.Id,
            user.FullName,
            user.Email,
            profile.ProfessionalTitle,
            profile.Headline,
            profile.Biography,
            profile.ProfileImageUrl ?? user.ImageUrl,
            profile.VerificationStatus.ToString(),
            profile.IsVerified,
            profile.RejectionReason,
            profile.CalculateCompletionPercentage(),
            experiences,
            education,
            certifications,
            skills,
            links);
    }
}
