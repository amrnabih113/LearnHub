using LearnHub.Domain.Identity;

namespace LearnHub.Application.Common.Interfaces.Authentication;

public interface IOtpProvider
{
    string GenerateOtp();

    bool ValidateOtp(
        string otp,

        string expectedOtp);

    string HashOtp(string otp);
}