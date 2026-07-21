namespace LearnHub.Application.common.Interfaces;

public interface INotificationService
{
    Task notifyAsync(string to, string subject, string body, CancellationToken cancellationToken = default);
   
}