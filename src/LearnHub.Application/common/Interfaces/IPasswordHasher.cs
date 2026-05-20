using LearnHub.Domain.Common.Results;

namespace LearnHub.Application.common.Interfaces;

public interface IPasswordHasher
{
    Result<string> HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
}

