using Wex.Purchases.Contracts.Requests;
using Wex.Purchases.Contracts.Results;

namespace Wex.Purchases.Application.Services;

public interface IPurchaseService
{
    Task<CreatePurchaseResult> CreateAsync(CreatePurchaseRequest request, CancellationToken cancellationToken = default);
    Task<CreatePurchaseResult?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
