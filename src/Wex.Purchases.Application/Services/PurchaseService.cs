using Wex.Purchases.Application.Repositories;
using Wex.Purchases.Contracts.Requests;
using Wex.Purchases.Contracts.Results;
using Wex.Purchases.Domain.Entities;

namespace Wex.Purchases.Application.Services;

public sealed class PurchaseService : IPurchaseService
{
    private readonly IPurchaseRepository _purchaseRepository;

    public PurchaseService(IPurchaseRepository purchaseRepository)
    {
        _purchaseRepository = purchaseRepository ?? throw new ArgumentNullException(nameof(purchaseRepository));
    }

    public async Task<CreatePurchaseResult> CreateAsync(CreatePurchaseRequest request, CancellationToken cancellationToken = default)
    {
        ValidateRequest(request);

        var roundedAmount = decimal.Round(request.AmountUsd, 2, MidpointRounding.AwayFromZero);

        var purchaseTransaction = new PurchaseTransaction(
            Guid.NewGuid(),
            request.Description,
            request.TransactionDate,
            roundedAmount);

        await _purchaseRepository.AddAsync(purchaseTransaction, cancellationToken);

        return new CreatePurchaseResult(
            purchaseTransaction.Id,
            purchaseTransaction.Description,
            purchaseTransaction.TransactionDate,
            purchaseTransaction.AmountUsd);
    }

    public async Task<CreatePurchaseResult?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id is required.", nameof(id));
        }

        var purchaseTransaction = await _purchaseRepository.GetByIdAsync(id, cancellationToken);
        if (purchaseTransaction is null)
        {
            return null;
        }

        return new CreatePurchaseResult(
            purchaseTransaction.Id,
            purchaseTransaction.Description,
            purchaseTransaction.TransactionDate,
            purchaseTransaction.AmountUsd);
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
}
