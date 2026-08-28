using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Security;

namespace LearnHub.Infrastructure.Services;

public sealed class SecurityAuditService(IAppDbContext context) : ISecurityAuditService
{
    private readonly IAppDbContext _context = context;

    public async Task LogAsync(
        Guid? actorUserId,
        string action,
        Guid? targetUserId = null,
        string? targetResourceId = null,
        string details = "",
        string? ipAddress = null,
        string? userAgent = null,
        CancellationToken cancellationToken = default)
    {
        var log = SecurityAuditLog.Create(
            actorUserId,
            action,
            targetUserId,
            targetResourceId,
            details,
            ipAddress,
            userAgent);

        await _context.SecurityAuditLogs.AddAsync(log, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
