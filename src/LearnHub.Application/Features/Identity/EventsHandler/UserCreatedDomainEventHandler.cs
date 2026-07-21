
using LearnHub.Application.Features.Identity.Commands.ResendVerificationEmail;
using LearnHub.Domain.Identity.Events;
using MediatR;

namespace LearnHub.Application.Features.Identity.Events;

public sealed class UserCreatedDomainEventHandler(
    ISender sender)
    : INotificationHandler<UserCreatedDomainEvent>
{
    private readonly ISender _sender = sender;

    public async Task Handle(
        UserCreatedDomainEvent notification,
        CancellationToken cancellationToken)
    {
        await _sender.Send(
            new SendVerificationEmailCommand(
                notification.Email),
            cancellationToken);
    }
}