namespace LearnHub.Application.Common.Interfaces.Notifications;

public interface IEmailService
{
    Task SendAsync(
        string to,
        string subject,
        string body,
        CancellationToken cancellationToken = default);
}