using LearnHub.Domain.Common;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Identity;

namespace LearnHub.Domain.Instructor;

public sealed class InstructorProfile : AuditableEntity
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = default!;

    public string? ProfessionalTitle { get; private set; }
    public string? Headline { get; private set; }
    public string? Biography { get; private set; }
    public string? ProfileImageUrl { get; private set; }
    public InstructorVerificationStatus VerificationStatus { get; private set; }
    public string? RejectionReason { get; private set; }
    public bool IsVerified => VerificationStatus == InstructorVerificationStatus.Approved;

    private readonly List<InstructorExperience> _experiences = [];
    public IReadOnlyCollection<InstructorExperience> Experiences => _experiences.AsReadOnly();

    private readonly List<InstructorEducation> _education = [];
    public IReadOnlyCollection<InstructorEducation> Education => _education.AsReadOnly();

    private readonly List<InstructorCertification> _certifications = [];
    public IReadOnlyCollection<InstructorCertification> Certifications => _certifications.AsReadOnly();

    private readonly List<InstructorSkill> _skills = [];
    public IReadOnlyCollection<InstructorSkill> Skills => _skills.AsReadOnly();

    private readonly List<InstructorLanguage> _languages = [];
    public IReadOnlyCollection<InstructorLanguage> Languages => _languages.AsReadOnly();

    private readonly List<InstructorLink> _links = [];
    public IReadOnlyCollection<InstructorLink> Links => _links.AsReadOnly();

    private InstructorProfile() { }

    private InstructorProfile(
        Guid id,
        Guid userId,
        string? professionalTitle = null,
        string? headline = null,
        string? biography = null,
        string? profileImageUrl = null,
        InstructorVerificationStatus verificationStatus = InstructorVerificationStatus.Pending) : base(id)
    {
        UserId = userId;
        ProfessionalTitle = professionalTitle;
        Headline = headline;
        Biography = biography;
        ProfileImageUrl = profileImageUrl;
        VerificationStatus = verificationStatus;
    }

    public static Result<InstructorProfile> Create(
        Guid userId,
        string? professionalTitle = null,
        string? headline = null,
        string? biography = null)
    {
        if (userId == Guid.Empty)
        {
            return Error.Validation("InstructorProfile.UserIdRequired", "User ID is required.");
        }

        return new InstructorProfile(
            Guid.NewGuid(),
            userId,
            professionalTitle?.Trim(),
            headline?.Trim(),
            biography?.Trim(),
            null,
            InstructorVerificationStatus.Pending);
    }

    public Result<Updated> UpdateBasicInfo(
        string? professionalTitle,
        string? headline,
        string? biography)
    {
        ProfessionalTitle = professionalTitle?.Trim();
        Headline = headline?.Trim();
        Biography = biography?.Trim();
        UpdatedAtUtc = DateTimeOffset.UtcNow;
        return Result.Updated;
    }

    public Result<Updated> UpdateProfileImage(string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            return Error.Validation("InstructorProfile.ImageUrlRequired", "Image URL is required.");
        }

        ProfileImageUrl = imageUrl.Trim();
        UpdatedAtUtc = DateTimeOffset.UtcNow;
        return Result.Updated;
    }

    public Result<Updated> Approve()
    {
        VerificationStatus = InstructorVerificationStatus.Approved;
        RejectionReason = null;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
        return Result.Updated;
    }

    public Result<Updated> Reject(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            return Error.Validation("InstructorProfile.RejectionReasonRequired", "Rejection reason is required.");
        }

        VerificationStatus = InstructorVerificationStatus.Rejected;
        RejectionReason = reason.Trim();
        UpdatedAtUtc = DateTimeOffset.UtcNow;
        return Result.Updated;
    }

    public Result<Updated> AddExperience(InstructorExperience experience)
    {
        if (experience is null) return Error.Validation("InstructorProfile.NullExperience", "Experience cannot be null.");
        _experiences.Add(experience);
        UpdatedAtUtc = DateTimeOffset.UtcNow;
        return Result.Updated;
    }

    public Result<Updated> RemoveExperience(Guid experienceId)
    {
        var item = _experiences.FirstOrDefault(e => e.Id == experienceId);
        if (item != null)
        {
            _experiences.Remove(item);
            UpdatedAtUtc = DateTimeOffset.UtcNow;
        }
        return Result.Updated;
    }

    public Result<Updated> AddEducation(InstructorEducation education)
    {
        if (education is null) return Error.Validation("InstructorProfile.NullEducation", "Education cannot be null.");
        _education.Add(education);
        UpdatedAtUtc = DateTimeOffset.UtcNow;
        return Result.Updated;
    }

    public Result<Updated> RemoveEducation(Guid educationId)
    {
        var item = _education.FirstOrDefault(e => e.Id == educationId);
        if (item != null)
        {
            _education.Remove(item);
            UpdatedAtUtc = DateTimeOffset.UtcNow;
        }
        return Result.Updated;
    }

    public Result<Updated> AddCertification(InstructorCertification certification)
    {
        if (certification is null) return Error.Validation("InstructorProfile.NullCertification", "Certification cannot be null.");
        _certifications.Add(certification);
        UpdatedAtUtc = DateTimeOffset.UtcNow;
        return Result.Updated;
    }

    public Result<Updated> RemoveCertification(Guid certificationId)
    {
        var item = _certifications.FirstOrDefault(c => c.Id == certificationId);
        if (item != null)
        {
            _certifications.Remove(item);
            UpdatedAtUtc = DateTimeOffset.UtcNow;
        }
        return Result.Updated;
    }

    public Result<Updated> AddSkill(InstructorSkill skill)
    {
        if (skill is null) return Error.Validation("InstructorProfile.NullSkill", "Skill cannot be null.");
        if (!_skills.Any(s => string.Equals(s.SkillName, skill.SkillName, StringComparison.OrdinalIgnoreCase)))
        {
            _skills.Add(skill);
            UpdatedAtUtc = DateTimeOffset.UtcNow;
        }
        return Result.Updated;
    }

    public Result<Updated> RemoveSkill(Guid skillId)
    {
        var item = _skills.FirstOrDefault(s => s.Id == skillId);
        if (item != null)
        {
            _skills.Remove(item);
            UpdatedAtUtc = DateTimeOffset.UtcNow;
        }
        return Result.Updated;
    }

    public Result<Updated> AddLink(InstructorLink link)
    {
        if (link is null) return Error.Validation("InstructorProfile.NullLink", "Link cannot be null.");
        _links.Add(link);
        UpdatedAtUtc = DateTimeOffset.UtcNow;
        return Result.Updated;
    }

    public Result<Updated> RemoveLink(Guid linkId)
    {
        var item = _links.FirstOrDefault(l => l.Id == linkId);
        if (item != null)
        {
            _links.Remove(item);
            UpdatedAtUtc = DateTimeOffset.UtcNow;
        }
        return Result.Updated;
    }

    public int CalculateCompletionPercentage()
    {
        int score = 0;
        int maxScore = 6;

        if (!string.IsNullOrWhiteSpace(ProfessionalTitle)) score++;
        if (!string.IsNullOrWhiteSpace(Headline)) score++;
        if (!string.IsNullOrWhiteSpace(Biography)) score++;
        if (!string.IsNullOrWhiteSpace(ProfileImageUrl)) score++;
        if (_skills.Count > 0) score++;
        if (_experiences.Count > 0 || _education.Count > 0) score++;

        return (int)Math.Round((double)score / maxScore * 100);
    }
}
