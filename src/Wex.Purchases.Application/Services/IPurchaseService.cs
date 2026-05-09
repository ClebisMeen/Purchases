using Wex.Purchases.Contracts.Requests;
using Wex.Purchases.Contracts.Results;
using Wex.Purchases.Application.Requests;
using Wex.Purchases.Application.Results;

namespace Wex.Purchases.Application.Services;

public interface IPurchaseService
{
    Task<CreatePurchaseResult> CreateAsync(CreatePurchaseRequest request, CancellationToken cancellationToken = default);
    Task<GetPurchaseConvertedResult?> GetByIdAsync(
        GetPurchaseConvertedRequest request,
        CancellationToken cancellationToken = default);
}
