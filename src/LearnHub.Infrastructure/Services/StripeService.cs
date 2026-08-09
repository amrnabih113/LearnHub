using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Common.Results;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace LearnHub.Infrastructure.Services;

public sealed class StripeService : IStripeService
{
    private readonly StripeSettings _settings;

    public string ProviderName => "Stripe";

    public StripeService(IOptions<StripeSettings> options)
    {
        _settings = options.Value;
        StripeConfiguration.ApiKey = _settings.SecretKey;
    }

    public async Task<Result<CheckoutSessionResult>> CreateCheckoutSessionAsync(
        CreateCheckoutSessionArgs args,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var unitAmount = (long)(args.Amount * 100);
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = ["card"],
                CustomerEmail = args.UserEmail,
                LineItems =
                [
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = unitAmount,
                            Currency = args.Currency.ToLowerInvariant(),
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = args.ItemTitle,
                                Description = $"{args.PaymentType} for {args.ItemTitle}"
                            }
                        },
                        Quantity = 1
                    }
                ],
                Mode = args.PaymentType == Application.common.Interfaces.PaymentType.SubscriptionPurchase ? "subscription" : "payment",
                SuccessUrl = args.SuccessUrl,
                CancelUrl = args.CancelUrl,
                Metadata = args.Metadata ?? new Dictionary<string, string>()
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options, cancellationToken: cancellationToken);

            return new CheckoutSessionResult(
                SessionId: session.Id,
                CheckoutUrl: session.Url,
                PaymentIntentId: session.PaymentIntentId,
                CustomerId: session.CustomerId);
        }
        catch (StripeException ex)
        {
            return Error.Failure("Stripe.CheckoutError", ex.Message);
        }
    }
}
