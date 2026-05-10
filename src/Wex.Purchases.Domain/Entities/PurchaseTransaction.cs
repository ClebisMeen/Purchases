namespace Wex.Purchases.Domain.Entities;

/// <summary>
/// Represents a purchase transaction stored by the application.
/// </summary>
public class PurchaseTransaction : Entity, IAggregateRoot
{
    /// <summary>
    /// Gets the purchase description.
    /// </summary>
    public string Description { get; private set; }

    /// <summary>
    /// Gets the date when the purchase occurred.
    /// </summary>
    public DateTime TransactionDate { get; private set; }

    /// <summary>
    /// Gets the purchase amount in USD.
    /// </summary>
    public decimal AmountUsd { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PurchaseTransaction"/> class.
    /// </summary>
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
