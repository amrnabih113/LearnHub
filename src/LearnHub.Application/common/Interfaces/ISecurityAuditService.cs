namespace LearnHub.Application.common.Interfaces;

public interface ISecurityAuditService
{
    Task LogAsync(
        Guid? actorUserId,
        string action,
        Guid? targetUserId = null,
        string? targetResourceId = null,
        string details = "",
        string? ipAddress = null,
        string? userAgent = null,
        CancellationToken cancellationToken = default);
}
