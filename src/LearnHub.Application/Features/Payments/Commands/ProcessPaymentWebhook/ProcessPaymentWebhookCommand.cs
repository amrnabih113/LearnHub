using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Payments.Commands.ProcessPaymentWebhook;

public sealed record ProcessPaymentWebhookCommand(
    string JsonPayload,
    string? SignatureHeader) : IRequest<Result<Updated>>;
