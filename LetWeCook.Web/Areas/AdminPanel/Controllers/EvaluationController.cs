using LetWeCook.Application.Exceptions;
using LetWeCook.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LetWeCook.Areas.AdminPanel.Controllers;

[Area("AdminPanel")]
[Authorize(Roles = "Admin")]
public class EvaluationController : Controller
{
    private readonly IRequestService _requestService;
    public EvaluationController(IRequestService requestService)
    {
        _requestService = requestService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost("/api/evaluation/accept/{newRefId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AcceptRequestByReferenceId(Guid newRefId, string responseMessage, CancellationToken cancellationToken = default)
    {
        try
        {
            await _requestService.ApproveRequestByNewRefIdAsync(newRefId, responseMessage, cancellationToken);
            return Ok(new { message = "Request approved successfully." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = "Operation is not valid." + ex.Message, details = ex.Message });
        }
        catch (IngredientRetrievalException ex)
        {
            return NotFound(new { error = "Ingredient could not be retrieved.", details = ex.Message });
        }
        catch (RequestRetrievalException ex)
        {
            return NotFound(new { error = "Request could not be retrieved.", details = ex.Message });
        }
    }


    [HttpPost("/api/evaluation/reject/{newRefId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RejectRequestByReferenceId(Guid newRefId, string responseMessage, CancellationToken cancellationToken = default)
    {
        try
        {
            await _requestService.RejectRequestByNewRefIdAsync(newRefId, responseMessage, cancellationToken);
            return Ok(new { message = "Request rejected successfully." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = "Operation is not valid.", details = ex.Message });
        }
        catch (IngredientRetrievalException ex)
        {
            return NotFound(new { error = "Ingredient could not be retrieved.", details = ex.Message });
        }
        catch (RequestRetrievalException ex)
        {
            return NotFound(new { error = "Request could not be retrieved.", details = ex.Message });
        }
    }

}