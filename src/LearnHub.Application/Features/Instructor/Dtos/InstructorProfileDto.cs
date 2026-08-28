namespace LearnHub.Application.Features.Instructor.Dtos;

public sealed record InstructorLinkDto(
    Guid Id,
    string Title,
    string Url);

public sealed record InstructorExperienceDto(
    Guid Id,
    string JobTitle,
    string Company,
    string? Description,
    DateOnly StartDate,
    DateOnly? EndDate,
    bool IsCurrent,
    string? Location);

public sealed record InstructorEducationDto(
    Guid Id,
    string Institution,
    string Degree,
    string FieldOfStudy,
    DateOnly StartDate,
    DateOnly? EndDate,
    string? Description);

public sealed record InstructorCertificationDto(
    Guid Id,
    string Name,
    string IssuingOrganization,
    DateOnly IssueDate,
    DateOnly? ExpirationDate,
    string? CredentialId,
    string? CredentialUrl);

public sealed record InstructorProfileDto(
    Guid UserId,
    string FullName,
    string Email,
    string? ProfessionalTitle,
    string? Headline,
    string? Biography,
    string? ProfileImageUrl,
    string VerificationStatus,
    bool IsVerified,
    string? RejectionReason,
    int CompletionPercentage,
    IReadOnlyList<InstructorExperienceDto> Experiences,
    IReadOnlyList<InstructorEducationDto> Education,
    IReadOnlyList<InstructorCertificationDto> Certifications,
    IReadOnlyList<string> Skills,
    IReadOnlyList<InstructorLinkDto> Links);
