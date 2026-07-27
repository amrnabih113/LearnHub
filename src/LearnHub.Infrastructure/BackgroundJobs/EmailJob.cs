using Hangfire;
using LearnHub.Application.Common.Interfaces;
using LearnHub.Application.Common.Models;

namespace LearnHub.Infrastructure.BackgroundJobs;

public sealed class EmailJob : IEmailJob
{
    private readonly IEmailService _emailService;
    private readonly IEmailTemplateService _templateService;


    public EmailJob(
        IEmailService emailService,
        IEmailTemplateService templateService)
    {
        _emailService = emailService;
        _templateService = templateService;
    }

    [AutomaticRetry(
        Attempts = 5,
        DelaysInSeconds = new[]
        {
        10,
        30,
        60,
        300,
        600
        })]
    public async Task SendAsync(
            EmailMessage message,
            CancellationToken cancellationToken = default)
    {
        var body = _templateService.Render(
            message.Template,
            message.Data);


        await _emailService.SendAsync(
            message.To,
            message.Subject,
            body,
            cancellationToken);
    }
}
