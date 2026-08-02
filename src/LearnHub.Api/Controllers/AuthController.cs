using LearnHub.Application.Features.Identity.Commands.Login;
using LearnHub.Application.Features.Identity.Commands.Register;
using LearnHub.Application.Features.Identity.Commands.VerifyEmail;
using LearnHub.Application.Features.Identity.Commands.ResendVerificationEmail;
using LearnHub.Application.Features.Identity.Commands.VerifyForgotPasswordOtp;
using LearnHub.Application.Features.Identity.Commands.ResetPassword;
using LearnHub.Application.Features.Identity.Commands.RefreshToken;
using LearnHub.Application.Features.Identity;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using LearnHub.Application.Features.Identity.Commands.ForgotPassword;


namespace LearnHub.Api.Controllers;


[Route("api/v1/auth")]
public sealed class AuthController(ISender sender) : BaseController
{
    private const string RefreshTokenCookieName = "refreshToken";

    private readonly ISender _sender = sender;



    [HttpPost("register")]
    public async Task<IActionResult> Register(
        RegisterCommand command,
        CancellationToken cancellationToken)
    {
        var result =
            await _sender.Send(
                command,
                cancellationToken);

        return HandleResult(result);
    }



    [HttpPost("login")]
    public async Task<IActionResult> Login(
        LoginCommand command,
        CancellationToken cancellationToken)
    {
        var result =
            await _sender.Send(
                command,
                cancellationToken);

        if (result.IsSuccess)
        {
            AppendRefreshTokenCookie(result.Value);
        }

        return HandleResult(result);
    }




    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail(
        VerifyEmailCommand command,
        CancellationToken cancellationToken)
    {
        var result =
            await _sender.Send(
                command,
                cancellationToken);

        return HandleResult(result);
    }





    [HttpPost("resend-verification-email")]
    public async Task<IActionResult> ResendVerificationEmail(
        SendVerificationEmailCommand command,
        CancellationToken cancellationToken)
    {
        var result =
            await _sender.Send(
                command,
                cancellationToken);

        return HandleResult(result);
    }





    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(
        ForgetPasswordCommand command,
        CancellationToken cancellationToken)
    {
        var result =
            await _sender.Send(
                command,
                cancellationToken);

        return HandleResult(result);
    }





    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(
        ResetPasswordCommand command,
        CancellationToken cancellationToken)
    {
        var result =
            await _sender.Send(
                command,
                cancellationToken);

        return HandleResult(result);
    }

    [HttpPost("forgot-password/verify-otp")]
    public async Task<IActionResult> VerifyForgotPasswordOtp(
        VerifyForgotPasswordOtpCommand command,
        CancellationToken cancellationToken)
    {
        var result =
            await _sender.Send(
                command,
                cancellationToken);

        return HandleResult(result);
    }





    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        // if (!Request.Cookies.TryGetValue(RefreshTokenCookieName, out var refreshToken))
        // {
        //     return Unauthorized();
        // }

        var command = new RefreshTokenCommand(request.RefreshToken, request.ExpiredToken);

        var result =
            await _sender.Send(
                command,
                cancellationToken);

        if (result.IsSuccess)
        {
            AppendRefreshTokenCookie(result.Value);
        }

        return HandleResult(result);
    }

    private void AppendRefreshTokenCookie(TokenResponse tokenResponse)
    {
        if (string.IsNullOrWhiteSpace(tokenResponse.RefreshToken))
        {
            return;
        }

        Response.Cookies.Append(
            RefreshTokenCookieName,
            tokenResponse.RefreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Strict,
                Expires = tokenResponse.RefreshTokenExpiresOnUtc
            });
    }

    public sealed record RefreshTokenRequest(string ExpiredToken, string RefreshToken);
}


