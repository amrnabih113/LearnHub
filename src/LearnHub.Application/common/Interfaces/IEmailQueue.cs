using LearnHub.Application.Common.Models;

namespace LearnHub.Application.Common.Interfaces;

public interface IEmailQueue
{
    ValueTask QueueAsync(
        EmailMessage message,
        CancellationToken cancellationToken = default);

    ValueTask<EmailMessage> DequeueAsync(
        CancellationToken cancellationToken);
}