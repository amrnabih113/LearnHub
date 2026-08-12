using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Subscriptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Subscriptions.Commands.ResumeSubscription;

public sealed class ResumeSubscriptionCommandHandler(
    IAppDbContext context,
    ICourseAccessService courseAccessService)
    : IRequestHandler<ResumeSubscriptionCommand, Result<Updated>>
{
    private readonly IAppDbContext _context = context;
    private readonly ICourseAccessService _courseAccessService = courseAccessService;

    public async Task<Result<Updated>> Handle(
        ResumeSubscriptionCommand request,
        CancellationToken cancellationToken)
    {
        var subscription = await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.StudentId == request.StudentId, cancellationToken);

        if (subscription is null)
        {
            return Error.NotFound("Subscription.NotFound", "No subscription found for user.");
        }

        if (subscription.Status == SubscriptionStatus.Cancelled && subscription.ExpiresAtUtc > DateTimeOffset.UtcNow)
        {
            subscription.EnableAutoRenew();
            subscription.Activate(DateTimeOffset.UtcNow);
        }
        else if (subscription.Status == SubscriptionStatus.Expired)
        {
            subscription.Activate(DateTimeOffset.UtcNow);
        }

        await _context.SaveChangesAsync(cancellationToken);
        await _courseAccessService.SynchronizeUserEnrollmentsAsync(request.StudentId, cancellationToken);

        return Result.Updated;
    }
}
