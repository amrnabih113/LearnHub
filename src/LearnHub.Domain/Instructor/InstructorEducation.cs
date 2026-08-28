using LearnHub.Domain.Common;
using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.Instructor;

public sealed class InstructorEducation : AuditableEntity
{
    public Guid InstructorProfileId { get; private set; }
    public string Institution { get; private set; } = default!;
    public string Degree { get; private set; } = default!;
    public string FieldOfStudy { get; private set; } = default!;
    public DateOnly StartDate { get; private set; }
    public DateOnly? EndDate { get; private set; }
    public string? Description { get; private set; }

    private InstructorEducation() { }

    private InstructorEducation(
        Guid id,
        Guid instructorProfileId,
        string institution,
        string degree,
        string fieldOfStudy,
        DateOnly startDate,
        DateOnly? endDate,
        string? description) : base(id)
    {
        InstructorProfileId = instructorProfileId;
        Institution = institution;
        Degree = degree;
        FieldOfStudy = fieldOfStudy;
        StartDate = startDate;
        EndDate = endDate;
        Description = description;
    }

    public static Result<InstructorEducation> Create(
        Guid id,
        Guid instructorProfileId,
        string institution,
        string degree,
        string fieldOfStudy,
        DateOnly startDate,
        DateOnly? endDate,
        string? description)
    {
        if (string.IsNullOrWhiteSpace(institution))
        {
            return Error.Validation("InstructorEducation.InstitutionRequired", "Institution is required.");
        }
        if (string.IsNullOrWhiteSpace(degree))
        {
            return Error.Validation("InstructorEducation.DegreeRequired", "Degree is required.");
        }

        return new InstructorEducation(
            id, instructorProfileId, institution.Trim(), degree.Trim(),
            fieldOfStudy?.Trim() ?? string.Empty, startDate, endDate, description?.Trim());
    }
}
