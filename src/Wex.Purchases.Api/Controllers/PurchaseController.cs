using Microsoft.AspNetCore.Mvc;
using Wex.Purchases.Application.Services;
using Wex.Purchases.Contracts.Requests;
using Wex.Purchases.Contracts.Results;

namespace Wex.Purchases.Api.Controllers;

[ApiController]
[Route("api/purchases")]
public sealed class PurchaseController : ControllerBase
{
    private const string GetPurchaseByIdRouteName = "GetPurchaseById";
    private readonly IPurchaseService _purchaseService;

    public PurchaseController(IPurchaseService purchaseService)
    {
        _purchaseService = purchaseService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreatePurchaseResult), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CreatePurchaseResult>> CreateAsync(
        [FromBody] CreatePurchaseRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _purchaseService.CreateAsync(request, cancellationToken);
            return CreatedAtRoute(GetPurchaseByIdRouteName, new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError("request", ex.Message);
            return ValidationProblem(ModelState);
        }
    }

    [HttpGet("{id:guid}", Name = GetPurchaseByIdRouteName)]
    [ProducesResponseType(typeof(CreatePurchaseResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CreatePurchaseResult>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _purchaseService.GetByIdAsync(id, cancellationToken);
            if (result is null)
            {
                return NotFound();
            }

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError("id", ex.Message);
            return ValidationProblem(ModelState);
        }
    }
}
