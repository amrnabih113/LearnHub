using LearnHub.Domain.Common;

namespace LearnHub.Domain.Security;

public sealed class SecurityAuditLog : Entity
{
    public Guid? ActorUserId { get; private set; }
    public string Action { get; private set; } = default!;
    public Guid? TargetUserId { get; private set; }
    public string? TargetResourceId { get; private set; }
    public string Details { get; private set; } = default!;
    public string? IpAddress { get; private set; }
    public string? UserAgent { get; private set; }
    public DateTimeOffset TimestampUtc { get; private set; }

    private SecurityAuditLog() { }

    private SecurityAuditLog(
        Guid id,
        Guid? actorUserId,
        string action,
        Guid? targetUserId,
        string? targetResourceId,
        string details,
        string? ipAddress,
        string? userAgent,
        DateTimeOffset timestampUtc) : base(id)
    {
        ActorUserId = actorUserId;
        Action = action;
        TargetUserId = targetUserId;
        TargetResourceId = targetResourceId;
        Details = details;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        TimestampUtc = timestampUtc;
    }

    public static SecurityAuditLog Create(
        Guid? actorUserId,
        string action,
        Guid? targetUserId = null,
        string? targetResourceId = null,
        string details = "",
        string? ipAddress = null,
        string? userAgent = null)
    {
        return new SecurityAuditLog(
            Guid.NewGuid(),
            actorUserId,
            action,
            targetUserId,
            targetResourceId,
            details,
            ipAddress,
            userAgent,
            DateTimeOffset.UtcNow);
    }
}
