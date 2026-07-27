using LearnHub.Application.Common.Models;

namespace LearnHub.Application.Common.Interfaces;


public interface IBackgroundJobService
{
    void QueueEmail(EmailMessage message);
}