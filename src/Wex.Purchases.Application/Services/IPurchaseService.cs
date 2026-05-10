namespace Wex.Purchases.Application.Services;

public interface IPurchaseService
{
    Task<CreatePurchaseResult> CreateAsync(CreatePurchaseRequest request, CancellationToken cancellationToken = default);
    Task<GetPurchaseConvertedResult?> GetByIdAsync(
        GetPurchaseConvertedRequest request,
        CancellationToken cancellationToken = default);
}
