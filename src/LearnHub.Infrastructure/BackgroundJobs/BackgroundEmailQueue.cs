using System.Threading.Channels;
using LearnHub.Application.Common.Interfaces;
using LearnHub.Application.Common.Models;
using LearnHub.Infrastructure.Email;

namespace LearnHub.Infrastructure.BackgroundJobs;

public sealed class BackgroundEmailQueue : IEmailQueue
{
    private readonly Channel<EmailMessage> _queue;

    public BackgroundEmailQueue()
    {
        _queue = Channel.CreateBounded<EmailMessage>(
                new BoundedChannelOptions(1000)
                {
                    FullMode = BoundedChannelFullMode.Wait
                });
    }

    public async ValueTask QueueAsync(EmailMessage message,
        CancellationToken cancellationToken = default)
    {
        await _queue.Writer.WriteAsync(message, cancellationToken);
    }

    public async ValueTask<EmailMessage> DequeueAsync(
        CancellationToken cancellationToken)
    {
        return await _queue.Reader.ReadAsync(cancellationToken);
    }


}
