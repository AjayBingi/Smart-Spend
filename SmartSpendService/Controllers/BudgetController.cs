using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartSpendService.DTOs;
using SmartSpendService.Helpers;
using SmartSpendService.Services;

namespace SmartSpendService.Controllers;

[ApiController]
[Route("api/budgets")]
[Authorize]
public class BudgetController : ControllerBase
{
    private readonly BudgetService _budgetService;

    public BudgetController(BudgetService budgetService)
    {
        _budgetService = budgetService;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var userId = ClaimsHelper.GetUserId(User);
        var budgets = _budgetService.GetAll(userId);
        return Ok(budgets);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var userId = ClaimsHelper.GetUserId(User);
        var budget = _budgetService.GetById(id, userId);
        if (budget == null)
            return NotFound(new { message = "Budget not found." });
        return Ok(budget);
    }

    [HttpPost]
    public IActionResult Create([FromBody] BudgetRequest request)
    {
        if (request.LimitAmount <= 0 || request.Month < 1 || request.Month > 12)
            return BadRequest(new { message = "Valid limit amount and month are required." });

        var userId = ClaimsHelper.GetUserId(User);
        _budgetService.Create(request, userId);
        return Ok(new { message = "Budget created successfully." });
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] BudgetRequest request)
    {
        if (request.LimitAmount <= 0 || request.Month < 1 || request.Month > 12)
            return BadRequest(new { message = "Valid limit amount and month are required." });

        var userId = ClaimsHelper.GetUserId(User);
        var updated = _budgetService.Update(id, request, userId);
        if (!updated)
            return NotFound(new { message = "Budget not found." });
        return Ok(new { message = "Budget updated successfully." });
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var userId = ClaimsHelper.GetUserId(User);
        var deleted = _budgetService.Delete(id, userId);
        if (!deleted)
            return NotFound(new { message = "Budget not found." });
        return Ok(new { message = "Budget deleted successfully." });
    }
}