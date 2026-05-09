namespace Wex.Purchases.Domain.Entities;

public class PurchaseTransaction : Entity, IAggregateRoot
{
    public string Description { get; private set; }
    public DateTime TransactionDate { get; private set; }
    public decimal AmountUsd { get; private set; }

    public PurchaseTransaction(Guid id, string description, DateTime transactionDate, decimal amountUsd) : base(id)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Description is required.", nameof(description));
        }

        if (description.Length > 50)
        {
            throw new ArgumentException("Description must be at most 50 characters.", nameof(description));
        }

        if (transactionDate == default)
        {
            throw new ArgumentException("TransactionDate must be a valid date.", nameof(transactionDate));
        }

        if (amountUsd <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amountUsd), "AmountUsd must be positive.");
        }

        Description = description;
        TransactionDate = transactionDate;
        AmountUsd = decimal.Round(amountUsd, 2, MidpointRounding.AwayFromZero);
    }
}
