using LearnHub.Domain.Common;
using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.Instructor;

public sealed class InstructorSkill : AuditableEntity
{
    public Guid InstructorProfileId { get; private set; }
    public string SkillName { get; private set; } = default!;

    private InstructorSkill() { }

    private InstructorSkill(Guid id, Guid instructorProfileId, string skillName) : base(id)
    {
        InstructorProfileId = instructorProfileId;
        SkillName = skillName;
    }

    public static Result<InstructorSkill> Create(Guid id, Guid instructorProfileId, string skillName)
    {
        if (string.IsNullOrWhiteSpace(skillName))
        {
            return Error.Validation("InstructorSkill.NameRequired", "Skill name is required.");
        }

        return new InstructorSkill(id, instructorProfileId, skillName.Trim());
    }
}
