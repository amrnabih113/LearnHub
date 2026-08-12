using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Purchasing.Commands.ProcessStripeWebhook;

public sealed record ProcessStripeWebhookCommand(
    string JsonPayload,
    string? SignatureHeader) : IRequest<Result<Updated>>;
