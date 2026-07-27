using LearnHub.Application.Common.Interfaces;
using LearnHub.Application.Common.Models;

namespace LearnHub.Infrastructure.Email.Templates;


public sealed class EmailTemplateService : IEmailTemplateService
{

    public string Render(
        EmailTemplate template,
        Dictionary<string, string> data)
    {
        return template switch
        {
            EmailTemplate.EmailVerification =>
                VerificationEmail(
                    data["Name"],
                    data["Otp"]),

            EmailTemplate.PasswordReset =>
                PasswordResetEmail(
                    data["Name"],
                    data["Otp"]),

            EmailTemplate.Welcome =>
                WelcomeEmail(
                    data["Name"]),

            _ =>
                throw new ArgumentOutOfRangeException(nameof(template))
        };
    }



    private string VerificationEmail(
        string name,
        string otp)
    {
        return $"""
        <!DOCTYPE html>
        <html>

        <body style="
            font-family:Arial,sans-serif;
            background:#f5f7fb;
            padding:40px;
        ">

        <div style="
            max-width:600px;
            margin:auto;
            background:white;
            padding:30px;
            border-radius:12px;
        ">

        <h2 style="color:#0E627F;">
            Welcome to LearnHub 🎓
        </h2>


        <p>
            Hello {name},
        </p>


        <p>
            Thanks for creating your LearnHub account.
            Verify your email using this code:
        </p>


        <div style="
            font-size:32px;
            font-weight:bold;
            letter-spacing:8px;
            text-align:center;
            color:#0E627F;
            margin:30px 0;
        ">
            {otp}
        </div>


        <p>
            This code expires in
            <strong>10 minutes</strong>.
        </p>


        <p>
            If you did not create this account,
            ignore this email.
        </p>


        <hr/>

        <small>
            LearnHub Team
        </small>


        </div>

        </body>
        </html>
        """;
    }



    private string PasswordResetEmail(
        string name,
        string otp)
    {
        return $"""
        <!DOCTYPE html>
        <html>

        <body style="
            font-family:Arial,sans-serif;
            background:#f5f7fb;
            padding:40px;
        ">


        <div style="
            max-width:600px;
            margin:auto;
            background:white;
            padding:30px;
            border-radius:12px;
        ">


        <h2>
            Reset your LearnHub password
        </h2>


        <p>
            Hello {name},
        </p>


        <p>
            Use this OTP to reset your password:
        </p>


        <h1 style="
            text-align:center;
            letter-spacing:8px;
            color:#0E627F;
        ">
            {otp}
        </h1>


        <p>
            This code expires in 10 minutes.
        </p>


        </div>


        </body>
        </html>
        """;
    }



    private string WelcomeEmail(string name)
    {
        return $"""
        <!DOCTYPE html>
        <html>

        <body style="
            font-family:Arial;
            padding:40px;
        ">

        <h2>
            Welcome to LearnHub {name} 🎓
        </h2>

        <p>
            Your account has been successfully verified.
        </p>

        </body>

        </html>
        """;
    }
}