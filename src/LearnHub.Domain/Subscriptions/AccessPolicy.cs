using LearnHub.Domain.Courses;

namespace LearnHub.Domain.Subscriptions;

public static class AccessPolicy
{
    public static bool CanAccessCourse(Course course, Subscription? subscription, bool purchased, TrialOffer? trialOffer, DateTimeOffset now)
    {
        if (course.Price.Amount == 0)
        {
            return true;
        }

        if (purchased)
        {
            return true;
        }

        if (trialOffer is not null && trialOffer.IsActive(now) && course.IsIncludedInSubscription && trialOffer.Tier >= course.RequiredSubscriptionTier)
        {
            return true;
        }

        if (subscription is null)
        {
            return false;
        }

        if (subscription.Status is not SubscriptionStatus.Active and not SubscriptionStatus.Trialing)
        {
            return false;
        }

        if (subscription.ExpiresAtUtc <= now)
        {
            return false;
        }

        if (!course.IsIncludedInSubscription)
        {
            return false;
        }

        return subscription.Tier >= course.RequiredSubscriptionTier;
    }
}