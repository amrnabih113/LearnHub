namespace LearnHub.Application.Common.Interfaces.Authentication;

public interface IOtpProvider
{
    string Generate();

    bool Validate(
        string otp,
        string expectedOtp);
}