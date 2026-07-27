using Hangfire;
using LearnHub.Application.Common.Interfaces;
using LearnHub.Application.Common.Models;

namespace LearnHub.Infrastructure.BackgroundJobs;

public sealed class HangfireBackgroundJobService
    : IBackgroundJobService
{

    private readonly IBackgroundJobClient _backgroundJobClient;


    public HangfireBackgroundJobService(
        IBackgroundJobClient backgroundJobClient)
    {
        _backgroundJobClient = backgroundJobClient;
    }


    public void QueueEmail(
        EmailMessage message)
    {
        _backgroundJobClient.Enqueue<IEmailJob>(
            job =>
            job.SendAsync(
                message,
                CancellationToken.None));
    }
}