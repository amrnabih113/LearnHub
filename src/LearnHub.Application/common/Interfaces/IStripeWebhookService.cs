using LearnHub.Domain.Common.Results;

namespace LearnHub.Application.common.Interfaces;

public interface IStripeWebhookService
{
    Task<Result<Updated>> ProcessWebhookAsync(string jsonPayload, string? signatureHeader, CancellationToken cancellationToken = default);
}
