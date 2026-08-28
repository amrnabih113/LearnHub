using LearnHub.Contracts.Auth.Requests;
using LearnHub.Application.Features.Identity;
using LearnHub.Application.Features.Identity.Commands.ForgotPassword;
using LearnHub.Application.Features.Identity.Commands.Login;
using LearnHub.Application.Features.Identity.Commands.RefreshToken;
using LearnHub.Application.Features.Identity.Commands.ResendVerificationEmail;
using LearnHub.Application.Features.Identity.Commands.ResetPassword;
using LearnHub.Application.Features.Identity.Commands.VerifyEmail;
using LearnHub.Application.Features.Identity.Commands.VerifyForgotPasswordOtp;
using LearnHub.Application.Features.Identity.Queries.GetCurrentUser;
using LearnHub.Application.Features.Identity.Queries.GetUserById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearnHub.Api.Controllers;

[Route("api/v1/auth")]
public sealed class AuthController(ISender sender) : BaseController
{
    private const string RefreshTokenCookieName = "refreshToken";
    private readonly ISender _sender = sender;

    [HttpPost("register/student")]
    public async Task<IActionResult> RegisterStudent(
        [FromBody] LearnHub.Contracts.Auth.Requests.RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var command = new LearnHub.Application.Features.Identity.Commands.RegisterStudent.RegisterStudentCommand(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Password,
            request.ConfirmPassword,
            request.PhoneNumber);

        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("register/instructor")]
    public async Task<IActionResult> RegisterInstructor(
        [FromBody] LearnHub.Contracts.Auth.Requests.RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var command = new LearnHub.Application.Features.Identity.Commands.RegisterInstructor.RegisterInstructorCommand(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Password,
            request.ConfirmPassword,
            PhoneNumber: request.PhoneNumber);

        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var command = new LoginCommand(request.Email, request.Password);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            AppendRefreshTokenCookie(result.Value);
        }

        return HandleResult(result);
    }

    [Authorize]
    [HttpGet("get-current-user")]
    public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetCurrentUserQuery(), cancellationToken);

        return HandleResult(result);
    }

    [HttpGet("get-user-by-id/{userId:guid}")]
    [Authorize]
    public async Task<IActionResult> GetUserById(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetUserByIdQuery(userId), cancellationToken);

        return HandleResult(result);
    }

    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail(
        [FromBody] VerifyEmailRequest request,
        CancellationToken cancellationToken)
    {
        var command = new VerifyEmailCommand(request.Email, request.Otp);
        var result = await _sender.Send(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpPost("resend-verification-email")]
    public async Task<IActionResult> ResendVerificationEmail(
        [FromBody] ResendVerificationEmailRequest request,
        CancellationToken cancellationToken)
    {
        var command = new SendVerificationEmailCommand(request.Email);
        var result = await _sender.Send(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ForgetPasswordCommand(request.Email);
        var result = await _sender.Send(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ResetPasswordCommand(request.ResetToken, request.NewPassword);
        var result = await _sender.Send(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpPost("forgot-password/verify-otp")]
    public async Task<IActionResult> VerifyForgotPasswordOtp(
        [FromBody] VerifyForgotPasswordOtpRequest request,
        CancellationToken cancellationToken)
    {
        var command = new VerifyForgotPasswordOtpCommand(request.Email, request.Otp);
        var result = await _sender.Send(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RefreshTokenCommand(request.RefreshToken, request.ExpiredToken);
        var result = await _sender.Send(command, cancellationToken);

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
}
