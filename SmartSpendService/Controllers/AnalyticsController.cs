using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartSpendService.Helpers;
using SmartSpendService.Services;

namespace SmartSpendService.Controllers;

[ApiController]
[Route("api/analytics")]
[Authorize]
public class AnalyticsController : ControllerBase
{
    private readonly AnalyticsService _analyticsService;

    public AnalyticsController(AnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    [HttpGet("by-category")]
    public IActionResult GetByCategory([FromQuery] int month, [FromQuery] int year)
    {
        if (month < 1 || month > 12)
            return BadRequest(new { message = "Valid month is required." });

        var userId = ClaimsHelper.GetUserId(User);
        var result = _analyticsService.GetByCategory(userId, month, year);
        return Ok(result);
    }

    [HttpGet("monthly")]
    public IActionResult GetMonthlyTrend([FromQuery] int month, [FromQuery] int year)
    {
        if (month < 1 || month > 12)
            return BadRequest(new { message = "Valid month is required." });

        if (year > DateTime.UtcNow.Year)
            return BadRequest(new { message = "Valid year is required." });

        var userId = ClaimsHelper.GetUserId(User);
        var result = _analyticsService.GetMonthlyTrend(userId, month, year);
        return Ok(result);
    }

    [HttpGet("budget-status")]
    public IActionResult GetBudgetStatus([FromQuery] int month, [FromQuery] int year)
    {
        if (month < 1 || month > 12)
            return BadRequest(new { message = "Valid month is required." });

        var userId = ClaimsHelper.GetUserId(User);
        var result = _analyticsService.GetBudgetStatus(userId, month, year);
        return Ok(result);
    }

    [HttpGet("anomalies")]
    public IActionResult GetAnomalies([FromQuery] int month, [FromQuery] int year)
    {
        if (month < 1 || month > 12)
            return BadRequest(new { message = "Valid month is required." });

        var userId = ClaimsHelper.GetUserId(User);
        var result = _analyticsService.GetAnomalies(userId, month, year);
        return Ok(result);
    }
}