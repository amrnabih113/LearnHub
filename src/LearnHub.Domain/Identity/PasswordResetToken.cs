using LearnHub.Domain.Common;
using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.Identity;

public sealed class PasswordResetToken : AuditableEntity
{
    public string Token { get; private set; } = default!;
    public Guid UserId { get; private set; }
    public DateTimeOffset ExpiresOnUtc { get; private set; }
    public DateTimeOffset? UsedAtUtc { get; private set; }
    public User User { get; private set; } = default!;

    public bool IsUsed => UsedAtUtc.HasValue;

    private PasswordResetToken()
    {
    }

    private PasswordResetToken(Guid id, string token, Guid userId, DateTimeOffset expiresOnUtc)
        : base(id)
    {
        Token = token;
        UserId = userId;
        ExpiresOnUtc = expiresOnUtc;
    }

    public void Consume()
    {
        UsedAtUtc = DateTimeOffset.UtcNow;
    }

    public static Result<PasswordResetToken> Create(Guid id, string token, Guid userId, DateTimeOffset expiresOnUtc)
    {
        if (id == Guid.Empty)
        {
            return PasswordResetTokenErrors.IdRequired;
        }

        if (string.IsNullOrWhiteSpace(token))
        {
            return PasswordResetTokenErrors.TokenRequired;
        }

        if (userId == Guid.Empty)
        {
            return PasswordResetTokenErrors.UserIdRequired;
        }

        if (expiresOnUtc <= DateTimeOffset.UtcNow)
        {
            return PasswordResetTokenErrors.ExpiryInvalid;
        }

        return new PasswordResetToken(id, token, userId, expiresOnUtc);
    }
}