namespace LearnHub.Contracts.Payments.Requests;

public sealed record CreateSubscriptionCheckoutRequest(
    Guid SubscriptionPlanId,
    string SuccessUrl,
    string CancelUrl);
