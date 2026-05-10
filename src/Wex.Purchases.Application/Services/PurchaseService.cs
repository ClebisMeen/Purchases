using System.Runtime.CompilerServices;
using FluentValidation;
using Wex.Purchases.Application.Currencies;
using Wex.Purchases.Application.Repositories;
using Wex.Purchases.Application.Validators;
using Wex.Purchases.Application.Requests;
using Wex.Purchases.Application.Results;
using Wex.Purchases.Contracts.Requests;
using Wex.Purchases.Contracts.Results;
using Wex.Purchases.Domain.Entities;
using Wex.Purchases.Domain.ValueObjects;
using Wex.Purchases.Infrastructure.Treasury.Requests;
using Wex.Purchases.Infrastructure.Treasury.Services;

[assembly: InternalsVisibleTo("Wex.Purchases.UnitTests")]
namespace Wex.Purchases.Application.Services;

public sealed class PurchaseService : IPurchaseService
{
    private readonly IPurchaseRepository _purchaseRepository;
    private readonly ITreasuryApiService _treasuryApiService;

    public PurchaseService(IPurchaseRepository purchaseRepository, ITreasuryApiService treasuryApiService)
    {
        _purchaseRepository = purchaseRepository ?? throw new ArgumentNullException(nameof(purchaseRepository));
        _treasuryApiService = treasuryApiService ?? throw new ArgumentNullException(nameof(treasuryApiService));
    }

    public async Task<CreatePurchaseResult> CreateAsync(
        CreatePurchaseRequest request, 
        CancellationToken cancellationToken = default)
    {
        ValidateRequest(request);

        var roundedAmount = Money.FromAmount(request.PurchaseAmount).Amount;

        var purchaseTransaction = new PurchaseTransaction(
            Guid.NewGuid(),
            request.Description,
            request.TransactionDate,
            roundedAmount);

        await _purchaseRepository.AddAsync(purchaseTransaction, cancellationToken);

        var result = new CreatePurchaseResult(
            purchaseTransaction.Id,
            purchaseTransaction.Description,
            purchaseTransaction.TransactionDate,
            purchaseTransaction.AmountUsd);

        return result;
    }

    public async Task<GetPurchaseConvertedResult?> GetByIdAsync(
        GetPurchaseConvertedRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateGetPurchaseConvertedRequest(request);

        var purchaseTransaction = await _purchaseRepository.GetByIdAsync(request.PurchaseId, cancellationToken);
        if (purchaseTransaction is null)
        {
            return null;
        }

        var supportedCurrency = SupportedCurrencyCatalog.GetByCountryCode(request.CountryCode);

        var result = await GetPurchaseConvertedAsync(
            purchaseTransaction,
            supportedCurrency.CurrencyDescription,
            cancellationToken);

        return result;
    }

    internal async Task<GetPurchaseConvertedResult> GetPurchaseConvertedAsync(
        PurchaseTransaction purchaseTransaction,
        string countryCurrencyDescription,
        CancellationToken cancellationToken)
    {
        var treasuryRequest = new GetTreasuryExchangeRateApiRequest(
            countryCurrencyDescription,
            DateOnly.FromDateTime(purchaseTransaction.TransactionDate));

        var exchangeRateApiResult = await _treasuryApiService.GetExchangeRateAsync(treasuryRequest, cancellationToken);
        if (exchangeRateApiResult is null)
            throw new InvalidOperationException(
                $"No exchange rate was found for '{countryCurrencyDescription}' on or before '{purchaseTransaction.TransactionDate:yyyy-MM-dd}'.");

        var amountConverted = Money
            .FromAmount(purchaseTransaction.AmountUsd)
            .Convert(exchangeRateApiResult.ExchangeRate)
            .Amount;

        var result = new GetPurchaseConvertedResult(
            purchaseTransaction.Id,
            purchaseTransaction.Description,
            purchaseTransaction.TransactionDate,
            purchaseTransaction.AmountUsd,
            exchangeRateApiResult.Country,
            exchangeRateApiResult.Currency,
            exchangeRateApiResult.CountryCurrencyDescription,
            exchangeRateApiResult.ExchangeRate,
            exchangeRateApiResult.RecordDate,
            amountConverted);

        return result;
    }

    internal static void ValidateRequest(CreatePurchaseRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var validationResult = new CreatePurchaseRequestValidator().Validate(request);
        if (validationResult.IsValid)
        {
            return;
        }

        var firstError = validationResult.Errors[0];

        if (firstError.PropertyName == nameof(CreatePurchaseRequest.PurchaseAmount) &&
            firstError.ErrorMessage == "PurchaseAmount must be positive.")
        {
            throw new ArgumentOutOfRangeException(nameof(request), firstError.ErrorMessage);
        }

        throw new ArgumentException(firstError.ErrorMessage, nameof(request));
    }

    internal static void ValidateGetPurchaseConvertedRequest(GetPurchaseConvertedRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var validationResult = new GetPurchaseConvertedRequestValidator().Validate(request);
        if (validationResult.IsValid)
        {
            return;
        }

        var firstError = validationResult.Errors[0];
        throw new ArgumentException(firstError.ErrorMessage, nameof(request));
    }

}
