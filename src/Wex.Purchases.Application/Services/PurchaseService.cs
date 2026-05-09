using Wex.Purchases.Application.Repositories;
using Wex.Purchases.Application.Requests;
using Wex.Purchases.Application.Results;
using Wex.Purchases.Contracts.Requests;
using Wex.Purchases.Contracts.Results;
using Wex.Purchases.Domain.Entities;
using Wex.Purchases.Infrastructure.Treasury.Requests;
using Wex.Purchases.Infrastructure.Treasury.Services;

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

        var roundedAmount = decimal.Round(request.AmountUsd, 2, MidpointRounding.AwayFromZero);

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

        var result = await GetPurchaseConvertedAsync(
            purchaseTransaction,
            request.CountryCurrencyDescription,
            cancellationToken);

        return result;
    }

    private async Task<GetPurchaseConvertedResult> GetPurchaseConvertedAsync(
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

        var amountConverted = CalculateAmountConverted(
            purchaseTransaction.AmountUsd,
            exchangeRateApiResult.ExchangeRate);

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

    private static void ValidateRequest(CreatePurchaseRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.Description))
        {
            throw new ArgumentException("Description is required.", nameof(request));
        }

        if (request.Description.Length > 50)
        {
            throw new ArgumentException("Description must be at most 50 characters.", nameof(request));
        }

        if (request.AmountUsd <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(request), "AmountUsd must be positive.");
        }

        if (request.TransactionDate == default)
        {
            throw new ArgumentException("TransactionDate must be a valid date.", nameof(request));
        }
    }

    private static void ValidateGetPurchaseConvertedRequest(GetPurchaseConvertedRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.PurchaseId == Guid.Empty)
        {
            throw new ArgumentException("PurchaseId is required.", nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.CountryCurrencyDescription))
        {
            throw new ArgumentException("CountryCurrencyDescription is required.", nameof(request));
        }
    }

    private static decimal CalculateAmountConverted(decimal amountUsd, decimal exchangeRate) =>
        decimal.Round(amountUsd * exchangeRate, 2, MidpointRounding.AwayFromZero);

}
