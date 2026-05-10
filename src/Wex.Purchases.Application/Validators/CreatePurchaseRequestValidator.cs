namespace Wex.Purchases.Application.Validators;

public sealed class CreatePurchaseRequestValidator : AbstractValidator<CreatePurchaseRequest>
{
    public CreatePurchaseRequestValidator()
    {
        RuleFor(request => request.Description)
            .NotEmpty()
            .WithMessage("Description is required.")
            .MaximumLength(50)
            .WithMessage("Description must be at most 50 characters.");

        RuleFor(request => request.TransactionDate)
            .NotEmpty()
            .WithMessage("TransactionDate must be a valid date.");

        RuleFor(request => request.PurchaseAmount)
            .GreaterThan(0)
            .WithMessage("PurchaseAmount must be positive.")
            .Must(HaveAtMostTwoDecimalPlaces)
            .WithMessage("PurchaseAmount must have at most 2 decimal places.");
    }

    private static bool HaveAtMostTwoDecimalPlaces(decimal amount)
    {
        return decimal.Round(amount, 2) == amount;
    }
}
