using System.Security.Claims;
using LearnHub.Application.Features.Quizzes.Commands.AddQuestion;
using LearnHub.Application.Features.Quizzes.Commands.CreateFinalExam;
using LearnHub.Application.Features.Quizzes.Commands.CreateSectionQuiz;
using LearnHub.Application.Features.Quizzes.Commands.PublishQuiz;
using LearnHub.Application.Features.Quizzes.Commands.SaveQuizAnswer;
using LearnHub.Application.Features.Quizzes.Commands.StartQuizAttempt;
using LearnHub.Application.Features.Quizzes.Commands.SubmitQuizAttempt;
using LearnHub.Application.Features.Quizzes.Queries.GetQuizAttemptResult;
using LearnHub.Application.Features.Quizzes.Queries.GetStudentQuizAttempts;
using LearnHub.Contracts.Quizzes.Requests;
using LearnHub.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearnHub.Api.Controllers;

[Route("api/v1")]
[Authorize]
public sealed class QuizzesController(ISender sender) : BaseController
{
    private readonly ISender _sender = sender;

    #region Student Endpoints

    [HttpPost("quizzes/{quizId:guid}/attempts")]
    public async Task<IActionResult> StartAttempt(Guid quizId, CancellationToken cancellationToken)
    {
        var studentId = GetCurrentUserId();
        var command = new StartQuizAttemptCommand(quizId, studentId);
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("quiz-attempts/{attemptId:guid}/answers/{questionId:guid}")]
    public async Task<IActionResult> SaveAnswer(
        Guid attemptId,
        Guid questionId,
        [FromBody] SaveAnswerRequest request,
        CancellationToken cancellationToken)
    {
        var studentId = GetCurrentUserId();
        var command = new SaveQuizAnswerCommand(attemptId, questionId, studentId, request.SelectedChoiceId, request.TextAnswer);
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("quiz-attempts/{attemptId:guid}/submit")]
    public async Task<IActionResult> SubmitAttempt(Guid attemptId, CancellationToken cancellationToken)
    {
        var studentId = GetCurrentUserId();
        var command = new SubmitQuizAttemptCommand(attemptId, studentId);
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("quiz-attempts/{attemptId:guid}/result")]
    public async Task<IActionResult> GetAttemptResult(Guid attemptId, CancellationToken cancellationToken)
    {
        var studentId = GetCurrentUserId();
        var query = new GetQuizAttemptResultQuery(attemptId, studentId);
        var result = await _sender.Send(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("quizzes/{quizId:guid}/attempts")]
    public async Task<IActionResult> GetStudentAttempts(Guid quizId, CancellationToken cancellationToken)
    {
        var studentId = GetCurrentUserId();
        var query = new GetStudentQuizAttemptsQuery(quizId, studentId);
        var result = await _sender.Send(query, cancellationToken);
        return HandleResult(result);
    }

    #endregion

    #region Instructor Management Endpoints

    [HttpPost("sections/{sectionId:guid}/quizzes")]
    [Authorize(Roles = $"{nameof(Role.Instructor)},{nameof(Role.Admin)}")]
    public async Task<IActionResult> CreateSectionQuiz(
        Guid sectionId,
        [FromBody] CreateSectionQuizRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateSectionQuizCommand(
            request.CourseId,
            sectionId,
            request.Title,
            request.Description,
            request.TimeLimitMinutes,
            request.MaxAttempts,
            request.PassPercentage);

        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("courses/{courseId:guid}/final-exam")]
    [Authorize(Roles = $"{nameof(Role.Instructor)},{nameof(Role.Admin)}")]
    public async Task<IActionResult> CreateFinalExam(
        Guid courseId,
        [FromBody] CreateFinalExamRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateFinalExamCommand(
            courseId,
            request.Title,
            request.Description,
            request.TimeLimitMinutes,
            request.MaxAttempts,
            request.PassPercentage);

        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("quizzes/{quizId:guid}/publish")]
    [Authorize(Roles = $"{nameof(Role.Instructor)},{nameof(Role.Admin)}")]
    public async Task<IActionResult> PublishQuiz(Guid quizId, CancellationToken cancellationToken)
    {
        var command = new PublishQuizCommand(quizId);
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("quizzes/{quizId:guid}/questions")]
    [Authorize(Roles = $"{nameof(Role.Instructor)},{nameof(Role.Admin)}")]
    public async Task<IActionResult> AddQuestion(
        Guid quizId,
        [FromBody] AddQuestionRequest request,
        CancellationToken cancellationToken)
    {
        var choiceInputs = request.Choices?.Select(c => new ChoiceInput(c.Text, c.IsCorrect)).ToList();
        var command = new AddQuestionCommand(
            quizId,
            request.Prompt,
            request.Type,
            request.Points,
            request.Order,
            choiceInputs,
            request.CorrectTextAnswer);

        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    #endregion

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var id) ? id : Guid.Empty;
    }
}
