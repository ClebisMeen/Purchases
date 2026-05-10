namespace Wex.Purchases.Application.Requests;

public sealed record GetPurchaseConvertedRequest(
    Guid PurchaseId,
    string CountryCode
);
