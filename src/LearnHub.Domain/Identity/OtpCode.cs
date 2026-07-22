using LearnHub.Domain.Common;

namespace LearnHub.Domain.Identity;

public sealed class OtpCode : AuditableEntity
{
    public Guid UserId { get; private set; }

    public string CodeHash { get; private set; } = default!;

    public DateTimeOffset ExpiresAtUtc { get; private set; }

    public DateTimeOffset? UsedAtUtc { get; private set; }

    public OtpPurpose Purpose { get; private set; }

    public User User { get; private set; } = default!;
    private OtpCode() { }

    private OtpCode(Guid id, Guid userId, string codeHash, DateTimeOffset expiresAtUtc, OtpPurpose purpose)
        : base(id)
    {
        UserId = userId;
        CodeHash = codeHash;
        ExpiresAtUtc = expiresAtUtc;
        Purpose = purpose;
    }

    public static LearnHub.Domain.Common.Results.Result<OtpCode> Create(Guid id, Guid userId, string codeHash, DateTimeOffset expiresAtUtc, OtpPurpose purpose)
    {
        if (string.IsNullOrWhiteSpace(codeHash))
        {
            return OtpCodeErrors.CodeRequired;
        }
        if (expiresAtUtc <= DateTimeOffset.UtcNow)
        {
            return OtpCodeErrors.ExpirationMustBeInTheFuture;
        }
        if (!Enum.IsDefined(typeof(OtpPurpose), purpose))
        {
            return OtpCodeErrors.InvalidPurpose;
        }
        if (userId == Guid.Empty)
        {
            return OtpCodeErrors.UserIdRequired;
        }
        return new OtpCode(id, userId, codeHash, expiresAtUtc, purpose);
    }
}
