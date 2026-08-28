using LearnHub.Application.common.Errors;
using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Common.Interfaces.Authentication;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Identity;
using LearnHub.Domain.Purchasing.ValueObjects;
using LearnHub.Domain.Subscriptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Identity.Commands.RegisterStudent;

public sealed record RegisterStudentCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string ConfirmPassword,
    string? PhoneNumber = null) : IRequest<Result<Created>>;

public sealed class RegisterStudentCommandHandler(
    IAppDbContext context,
    IPasswordHasher passwordHasher)
    : IRequestHandler<RegisterStudentCommand, Result<Created>>
{
    private readonly IAppDbContext _context = context;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;

    public async Task<Result<Created>> Handle(RegisterStudentCommand request, CancellationToken cancellationToken)
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

        var userResult = User.Create(
            id: Guid.NewGuid(),
            firstName: request.FirstName,
            lastName: request.LastName,
            email: request.Email,
            passwordHash: passwordHash.Value,
            role: Role.Student, // Hardcoded role boundary
            phoneNumber: request.PhoneNumber ?? string.Empty);

        if (userResult.IsError)
        {
            return userResult.Errors;
        }

        var user = userResult.Value;
        await _context.Users.AddAsync(user, cancellationToken);

        // Initialize free subscription
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
            user.Id,
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

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Created;
    }
}
