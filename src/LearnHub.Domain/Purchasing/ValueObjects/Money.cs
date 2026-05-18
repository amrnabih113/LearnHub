using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Purchasing.Enums;

namespace LearnHub.Domain.Purchasing.ValueObjects;

public sealed record Money
{
    public decimal Amount { get; }
    public string Currency { get; }

    private Money(decimal amount, string currency)
    {
        Amount = decimal.Round(amount, 2);
        Currency = currency;
    }

    public static Result<Money> Create(decimal amount, string currency)
    {
        if (string.IsNullOrWhiteSpace(currency))
        {
            return OrderErrors.CurrencyRequired;
        }

        if (currency.Trim().Length != 3)
        {
            return OrderErrors.InvalidCurrency;
        }

        if (amount < 0)
        {
            return OrderErrors.InvalidDiscount;
        }

        return new Money(amount, currency.Trim().ToUpperInvariant());
    }

    public static Money Zero(string currency) => new(0, currency.Trim().ToUpperInvariant());

    public Result<Money> Add(Money other)
    {
        if (!string.Equals(Currency, other.Currency, StringComparison.OrdinalIgnoreCase))
        {
            return OrderErrors.InvalidCurrency;
        }

        return new Money(Amount + other.Amount, Currency);
    }

    public Result<Money> Subtract(Money other)
    {
        if (!string.Equals(Currency, other.Currency, StringComparison.OrdinalIgnoreCase))
        {
            return OrderErrors.InvalidCurrency;
        }

        var amount = Amount - other.Amount;
        if (amount < 0)
        {
            amount = 0;
        }

        return new Money(amount, Currency);
    }

    public Money Multiply(int factor)
    {
        return new Money(Amount * factor, Currency);
    }
}
