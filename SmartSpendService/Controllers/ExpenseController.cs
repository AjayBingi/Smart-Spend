using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartSpendService.DTOs;
using SmartSpendService.Helpers;
using SmartSpendService.Services;

namespace SmartSpendService.Controllers;

[ApiController]
[Route("api/expenses")]
[Authorize]
public class ExpenseController : ControllerBase
{
    private readonly ExpenseService _expenseService;

    public ExpenseController(ExpenseService expenseService)
    {
        _expenseService = expenseService;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var userId = ClaimsHelper.GetUserId(User);
        var expenses = _expenseService.GetAll(userId);
        return Ok(expenses);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var userId = ClaimsHelper.GetUserId(User);
        var expense = _expenseService.GetById(id, userId);
        if (expense == null)
            return NotFound(new { message = "Expense not found." });
        return Ok(expense);
    }

    [HttpPost]
    public IActionResult Create([FromBody] ExpenseRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title) || request.Amount <= 0)
            return BadRequest(new { message = "Title and valid amount are required." });

        var userId = ClaimsHelper.GetUserId(User);
        _expenseService.Create(request, userId);
        return Ok(new { message = "Expense created successfully." });
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] ExpenseRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title) || request.Amount <= 0)
            return BadRequest(new { message = "Title and valid amount are required." });

        var userId = ClaimsHelper.GetUserId(User);
        var updated = _expenseService.Update(id, request, userId);
        if (!updated)
            return NotFound(new { message = "Expense not found." });
        return Ok(new { message = "Expense updated successfully." });
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var userId = ClaimsHelper.GetUserId(User);
        var deleted = _expenseService.Delete(id, userId);
        if (!deleted)
            return NotFound(new { message = "Expense not found." });
        return Ok(new { message = "Expense deleted successfully." });
    }
}