using LearnHub.Domain.Identity;

namespace LearnHub.Application.Common.Interfaces.Authentication;

public interface IOtpProvider
{
    string GenerateOtp(OtpPurpose otpPurpose);

    bool ValidateOtp(
        string otp,

        string expectedOtp);

    string HashOtp(string otp);
}