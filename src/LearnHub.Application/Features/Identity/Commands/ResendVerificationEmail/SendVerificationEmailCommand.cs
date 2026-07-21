namespace LearnHub.Application.Features.Identity.Commands.ResendVerificationEmail;

using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Common.Results.Abstractions;
using MediatR;

public sealed record SendVerificationEmailCommand(
    string Email) : IRequest<Result<Created>>;
