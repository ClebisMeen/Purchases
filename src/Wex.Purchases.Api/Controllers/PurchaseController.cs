using Microsoft.AspNetCore.Mvc;
using Wex.Purchases.Application.Requests;
using Wex.Purchases.Application.Results;
using Wex.Purchases.Application.Services;
using Wex.Purchases.Contracts.Requests;
using Wex.Purchases.Contracts.Results;

namespace Wex.Purchases.Api.Controllers;

[ApiController]
[Route("api/purchases")]
public sealed class PurchaseController : ControllerBase
{
    private readonly IPurchaseService _purchaseService;

    public PurchaseController(IPurchaseService purchaseService)
    {
        _purchaseService = purchaseService;
    }

    [HttpPost]
    public async Task<ActionResult<CreatePurchaseResult>> CreateAsync(
        [FromBody] CreatePurchaseRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _purchaseService.CreateAsync(request, cancellationToken);
        return Ok(result);
    }

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
