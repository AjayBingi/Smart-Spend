using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartSpendService.DTOs;
using SmartSpendService.Helpers;
using SmartSpendService.Services;

namespace SmartSpendService.Controllers;

[ApiController]
[Route("api/ai")]
[Authorize]
public class AIController : ControllerBase
{
    private readonly AIService _aiService;

    public AIController(AIService aiService)
    {
        _aiService = aiService;
    }

    [HttpPost("parse-expense")]
    public async Task<IActionResult> ParseExpense([FromBody] ParseExpenseRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
            return BadRequest(new { message = "Text is required." });

        var result = await _aiService.ParseExpense(request.Text);
        return Ok(result);
    }

    [HttpPost("monthly-report")]
    public async Task<IActionResult> MonthlyReport([FromBody] MonthlyReportRequest request)
    {
        if (request.Month < 1 || request.Month > 12)
            return BadRequest(new { message = "Valid month is required." });

        var userId = ClaimsHelper.GetUserId(User);
        var result = await _aiService.MonthlyReport(userId, request.Month, request.Year);
        return Ok(new AITextResponse { Result = result });
    }

    [HttpPost("tips")]
    public async Task<IActionResult> SpendingTips()
    {
        var userId = ClaimsHelper.GetUserId(User);
        var result = await _aiService.SpendingTips(userId);
        return Ok(new AITextResponse { Result = result });
    }

    [HttpPost("investment-advice")]
    public async Task<IActionResult> InvestmentAdvice([FromBody] InvestmentRequest request)
    {
        if (request.Month < 1 || request.Month > 12)
            return BadRequest(new { message = "Valid month is required." });

        var userId = ClaimsHelper.GetUserId(User);

        try
        {
            var result = await _aiService.InvestmentAdvice(userId, request.Month, request.Year);
            return Ok(new AITextResponse { Result = result });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}