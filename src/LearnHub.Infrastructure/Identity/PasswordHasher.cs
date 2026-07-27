using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Common.Results;
using Microsoft.AspNetCore.Identity;


namespace LearnHub.Infrastructure.Identity;


public sealed class PasswordHasherService 
    : IPasswordHasher
{
    private readonly PasswordHasher<object> _hasher = new();


    public Result<string> HashPassword(string password)
    {
        if(string.IsNullOrWhiteSpace(password))
        {
            return Error.Validation(
                "Password.Required",
                "Password is required");
        }


        var hash = _hasher.HashPassword(
            new object(),
            password);


        return hash;
    }



    public bool VerifyPassword(
        string password,
        string passwordHash)
    {
        if(string.IsNullOrWhiteSpace(password)
            ||
           string.IsNullOrWhiteSpace(passwordHash))
        {
            return false;
        }


        var result = _hasher.VerifyHashedPassword(
            new object(),
            passwordHash,
            password);


        return result == PasswordVerificationResult.Success;
    }
}