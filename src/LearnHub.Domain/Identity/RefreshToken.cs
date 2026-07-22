

using LearnHub.Domain.Common;
using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.Identity;

public sealed class RefreshToken : AuditableEntity
{
    public string? Token { get; }
    public Guid? UserId { get; }
    public DateTimeOffset ExpiresOnUtc { get; }
    public DateTimeOffset? RevokedAtUtc { get; private set; }

    public User User { get; private set; } = default!;
    public bool IsRevoked => RevokedAtUtc.HasValue;

    public void Revoke()
    {
        RevokedAtUtc = DateTimeOffset.UtcNow;
    }
    private RefreshToken()
    { }

    private RefreshToken(Guid id, string? token, Guid? userId, DateTimeOffset expiresOnUtc)
        : base(id)
    {
        Token = token;
        UserId = userId;
        ExpiresOnUtc = expiresOnUtc;
    }

    public static Result<RefreshToken> Create(Guid id, string? token, Guid? userId, DateTimeOffset expiresOnUtc)
    {
        if (id == Guid.Empty)
        {
            return RefreshTokenErrors.IdRequired;
        }

        if (string.IsNullOrWhiteSpace(token))
        {
            return RefreshTokenErrors.TokenRequired;
        }

        if (userId == Guid.Empty)
        {
            return RefreshTokenErrors.UserIdRequired;
        }

        if (expiresOnUtc <= DateTimeOffset.UtcNow)
        {
            return RefreshTokenErrors.ExpiryInvalid;
        }

        return new RefreshToken(id, token, userId, expiresOnUtc);
    }
}