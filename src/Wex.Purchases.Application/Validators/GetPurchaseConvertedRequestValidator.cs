using FluentValidation;
using Wex.Purchases.Application.Requests;

namespace Wex.Purchases.Application.Validators;

public sealed class GetPurchaseConvertedRequestValidator : AbstractValidator<GetPurchaseConvertedRequest>
{
    public GetPurchaseConvertedRequestValidator()
    {
        RuleFor(request => request.PurchaseId)
            .NotEmpty()
            .WithMessage("PurchaseId is required.");

        RuleFor(request => request.CountryCurrencyDescription)
            .NotEmpty()
            .WithMessage("CountryCurrencyDescription is required.")
            .Must(BeSupportedCountryCurrencyDescriptionFormat)
            .WithMessage("CountryCurrencyDescription must match a supported Treasury API description format, for example: Brazil-Real.");
    }

    private static bool BeSupportedCountryCurrencyDescriptionFormat(string countryCurrencyDescription)
    {
        if (string.IsNullOrWhiteSpace(countryCurrencyDescription))
        {
            return false;
        }

        var segments = countryCurrencyDescription.Split('-', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length != 2)
        {
            return false;
        }

        return segments.All(segment => segment.All(character => char.IsLetter(character) || character == ' '));
    }
}
