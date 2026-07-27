using LearnHub.Application.Common.Interfaces;
using LearnHub.Infrastructure.Email.Templates;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LearnHub.Infrastructure.BackgroundJobs;

public sealed class EmailBackgroundService : BackgroundService
{
    private readonly IEmailQueue _queue;
    private readonly IEmailService _emailService;
    private readonly IEmailTemplateService _templateService;
    private readonly ILogger<EmailBackgroundService> _logger;

    public EmailBackgroundService(
        IEmailQueue queue,
        IEmailService emailService,
        IEmailTemplateService templateService,
        ILogger<EmailBackgroundService> logger)
    {
        _queue = queue;
        _emailService = emailService;
        _templateService = templateService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var email = await _queue.DequeueAsync(stoppingToken);

                var body = _templateService.Render(
                    email.Template,
                    email.Data);

                await _emailService.SendAsync(
                    email.To,
                    email.Subject,
                    body,
                    stoppingToken);

                _logger.LogInformation(
                    "Email sent successfully to {Email}",
                    email.To);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to process email from background queue.");
            }
        }
    }
}