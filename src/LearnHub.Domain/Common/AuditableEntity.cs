namespace LearnHub.Domain.Common;

public class AuditableEntity : Entity
{
    public DateTimeOffset CreatedAtUtc { get; set; }
    public DateTimeOffset? UpdatedAtUtc { get; set; }

    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
    protected AuditableEntity() : base()
    {
        CreatedAtUtc = DateTimeOffset.UtcNow;
    }

    protected AuditableEntity(Guid id) : base(id)
    {
        CreatedAtUtc = DateTimeOffset.UtcNow;
    }
}