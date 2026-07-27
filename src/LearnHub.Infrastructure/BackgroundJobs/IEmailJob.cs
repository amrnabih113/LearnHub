using LearnHub.Application.Common.Models;

namespace LearnHub.Infrastructure.BackgroundJobs;


public interface IEmailJob
{
    Task SendAsync(
        EmailMessage message,
        CancellationToken cancellationToken = default);
}


