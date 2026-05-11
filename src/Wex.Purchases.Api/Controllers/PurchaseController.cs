namespace Wex.Purchases.Api.Controllers;

/// <summary>
/// Exposes endpoints for creating purchases and retrieving converted purchase values.
/// </summary>
[ApiController]
[Route("api/purchases")]
public sealed class PurchaseController : ControllerBase
{
    private readonly IPurchaseService _purchaseService;

    /// <summary>
    /// Initializes a new instance of the <see cref="PurchaseController"/> class.
    /// </summary>
    public PurchaseController(IPurchaseService purchaseService)
    {
        _purchaseService = purchaseService;
    }

    /// <summary>
    /// Creates a new purchase transaction.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<CreatePurchaseResult>> CreateAsync(
        [FromBody] CreatePurchaseRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _purchaseService.CreateAsync(request, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets a purchase by identifier and returns its amount converted to the requested country currency.
    /// </summary>
    [HttpGet("{id}", Name = "GetPurchaseById")]
    public async Task<ActionResult<GetPurchaseConvertedResult>> GetByIdAsync(
        Guid id,
        [FromQuery] string countryCode,
        CancellationToken cancellationToken)
    {
        var request = new GetPurchaseConvertedRequest(id, countryCode);
        var result = await _purchaseService.GetByIdAsync(request, cancellationToken);
        return Ok(result);
    }
}
