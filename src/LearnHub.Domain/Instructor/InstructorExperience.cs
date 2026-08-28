using LearnHub.Domain.Common;
using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.Instructor;

public sealed class InstructorExperience : AuditableEntity
{
    public Guid InstructorProfileId { get; private set; }
    public string JobTitle { get; private set; } = default!;
    public string Company { get; private set; } = default!;
    public string? Description { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly? EndDate { get; private set; }
    public bool IsCurrent { get; private set; }
    public string? Location { get; private set; }

    private InstructorExperience() { }

    private InstructorExperience(
        Guid id,
        Guid instructorProfileId,
        string jobTitle,
        string company,
        string? description,
        DateOnly startDate,
        DateOnly? endDate,
        bool isCurrent,
        string? location) : base(id)
    {
        InstructorProfileId = instructorProfileId;
        JobTitle = jobTitle;
        Company = company;
        Description = description;
        StartDate = startDate;
        EndDate = endDate;
        IsCurrent = isCurrent;
        Location = location;
    }

    public static Result<InstructorExperience> Create(
        Guid id,
        Guid instructorProfileId,
        string jobTitle,
        string company,
        string? description,
        DateOnly startDate,
        DateOnly? endDate,
        bool isCurrent,
        string? location)
    {
        if (string.IsNullOrWhiteSpace(jobTitle))
        {
            return Error.Validation("InstructorExperience.JobTitleRequired", "Job title is required.");
        }
        if (string.IsNullOrWhiteSpace(company))
        {
            return Error.Validation("InstructorExperience.CompanyRequired", "Company is required.");
        }

        return new InstructorExperience(
            id, instructorProfileId, jobTitle.Trim(), company.Trim(),
            description?.Trim(), startDate, isCurrent ? null : endDate, isCurrent, location?.Trim());
    }
}
