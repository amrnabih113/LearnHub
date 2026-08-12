using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Subscriptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Subscriptions.Commands.CancelSubscription;

public sealed class CancelSubscriptionCommandHandler(
    IAppDbContext context,
    ICourseAccessService courseAccessService)
    : IRequestHandler<CancelSubscriptionCommand, Result<Updated>>
{
    private readonly IAppDbContext _context = context;
    private readonly ICourseAccessService _courseAccessService = courseAccessService;

    public async Task<Result<Updated>> Handle(
        CancelSubscriptionCommand request,
        CancellationToken cancellationToken)
    {
        var subscription = await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.StudentId == request.StudentId
                                   && (s.Status == SubscriptionStatus.Active || s.Status == SubscriptionStatus.Trialing), cancellationToken);

        if (subscription is null)
        {
            return Error.NotFound("Subscription.NotFound", "No active subscription found for user.");
        }

        var cancelResult = subscription.Cancel(DateTimeOffset.UtcNow);
        if (cancelResult.IsError)
        {
            return cancelResult.Errors;
        }

        await _context.SaveChangesAsync(cancellationToken);
        await _courseAccessService.SynchronizeUserEnrollmentsAsync(request.StudentId, cancellationToken);

        return Result.Updated;
    }
}
