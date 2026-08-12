using LearnHub.Application.common.Errors;
using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Common.Interfaces.Authentication;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Identity;
using LearnHub.Domain.Purchasing.ValueObjects;
using LearnHub.Domain.Subscriptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Identity.Commands.Register;

public class RegisterCommandHandler(IAppDbContext context,
                                    IPasswordHasher passwordHasher,
                                    ITokenProvider tokenProvider) : IRequestHandler<RegisterCommand, Result<Created>>
{
    private readonly IAppDbContext _context = context;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly ITokenProvider _tokenProvider = tokenProvider;

    public async Task<Result<Created>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (await _context.Users.AnyAsync(x => x.Email == request.Email, cancellationToken))
        {
            return ApplicationErrors.EmailAlreadyExists;
        }
        if (request.Password != request.ConfirmPassword)
        {
            return ApplicationErrors.PasswordsDontMatch;
        }

        var passwordHash = _passwordHasher.HashPassword(request.Password);
        if (passwordHash.IsError)
        {
            return passwordHash.Errors;
        }
        if (!Enum.IsDefined(typeof(Role), request.Role))
        {
            return ApplicationErrors.InvalidRole;
        }
        if (request.Role == Role.Admin)
        {
            return ApplicationErrors.AdminRoleUnauthorized;
        }
        var user = User.Create(
            id: Guid.NewGuid(),
            firstName: request.FirstName,
            lastName: request.LastName,
            email: request.Email,
            passwordHash: passwordHash.Value,
            role: request.Role,
            phoneNumber: request.PhoneNumber ?? string.Empty);

        if (user.IsError)
        {
            return user.Errors;
        }

        await _context.Users.AddAsync(user.Value, cancellationToken);

        if (request.Role == Role.Student)
        {
            var freePlan = await _context.SubscriptionPlans
                .FirstOrDefaultAsync(p => p.Tier == SubscriptionTier.Free, cancellationToken);

            if (freePlan is null)
            {
                var createPlanResult = SubscriptionPlan.Create(
                    Guid.NewGuid(),
                    "Free Default Plan",
                    SubscriptionTier.Free,
                    BillingCycle.Monthly,
                    Money.Create(0, "USD").Value);

                if (createPlanResult.IsSuccess)
                {
                    freePlan = createPlanResult.Value;
                    await _context.SubscriptionPlans.AddAsync(freePlan, cancellationToken);
                }
            }

            var now = DateTimeOffset.UtcNow;
            var defaultSubResult = Subscription.Create(
                Guid.NewGuid(),
                user.Value.Id,
                SubscriptionTier.Free,
                BillingCycle.Monthly,
                now,
                now.AddYears(100));

            if (defaultSubResult.IsSuccess)
            {
                var defaultSub = defaultSubResult.Value;
                defaultSub.Activate(now);
                await _context.Subscriptions.AddAsync(defaultSub, cancellationToken);
                if (freePlan is not null)
                {
                    _context.Entry(defaultSub).Property(s => s.SubscriptionPlanId).CurrentValue = freePlan.Id;
                }
            }
        }


        await _context.SaveChangesAsync(cancellationToken);
        return Result.Created;
    }
}