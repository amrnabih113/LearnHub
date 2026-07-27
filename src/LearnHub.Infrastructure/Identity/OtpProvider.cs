using System.Security.Cryptography;
using System.Text;

using LearnHub.Application.Common.Interfaces.Authentication;
using LearnHub.Domain.Identity;



namespace LearnHub.Infrastructure.Identity;


public sealed class OtpProvider
    : IOtpProvider
{


    public string GenerateOtp()
    {

        return RandomNumberGenerator
            .GetInt32(
                100000,
                999999)
            .ToString();
    }




    public bool ValidateOtp(
        string otp,
        string expectedOtp)
    {
        return otp == expectedOtp;
    }





    public string HashOtp(
        string otp)
    {

        using var sha =
            SHA256.Create();


        var bytes =
            Encoding.UTF8
            .GetBytes(otp);



        return Convert.ToBase64String(
            sha.ComputeHash(bytes));
    }


}