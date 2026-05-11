using Wex.Purchases.Application.Currencies;

namespace Wex.Purchases.Application.Validators;

/// <summary>
/// Validates requests used to retrieve converted purchase data.
/// </summary>
public sealed class GetPurchaseConvertedRequestValidator : AbstractValidator<GetPurchaseConvertedRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetPurchaseConvertedRequestValidator"/> class.
    /// </summary>
    public GetPurchaseConvertedRequestValidator()
    {
        RuleFor(request => request.PurchaseId)
            .NotEmpty()
            .WithMessage("PurchaseId is required.")
            .Must(BeValidPurchaseId)
            .WithMessage("PurchaseId must be a valid GUID.");

        RuleFor(request => request.CountryCode)
            .NotEmpty()
            .WithMessage("countryCode is required.")
            .Must(SupportedCurrencyCatalog.IsSupportedCountryCode)
            .WithMessage($"Invalid countryCode. Accepted values are: {SupportedCurrencyCatalog.AcceptedCountryCodes}.");
    }

    private static bool BeValidPurchaseId(Guid purchaseId)
    {
        return purchaseId != Guid.Empty;
    }
}
