using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.Purchasing.ValueObjects;

public sealed record TransactionId
{
    public string Value { get; }

    private TransactionId(string value)
    {
        Value = value;
    }

    public static Result<TransactionId> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return PaymentErrors.InvalidTransactionId;
        }

        return new TransactionId(value.Trim());
    }
}
