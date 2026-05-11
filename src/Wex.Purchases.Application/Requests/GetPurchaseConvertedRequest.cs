namespace Wex.Purchases.Application.Requests;

/// <summary>
/// Represents a request to retrieve a purchase converted to a supported country currency.
/// </summary>
public sealed record GetPurchaseConvertedRequest(
    Guid PurchaseId,
    string CountryCode
);
