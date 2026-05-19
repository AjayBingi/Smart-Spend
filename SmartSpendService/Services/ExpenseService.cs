using SmartSpendService.DTOs;
using SmartSpendService.Models;
using SmartSpendService.Repositories;

namespace SmartSpendService.Services;

public class ExpenseService
{
    private readonly ExpenseLibrary _expenseLibrary;

    public ExpenseService(ExpenseLibrary expenseLibrary)
    {
        _expenseLibrary = expenseLibrary;
    }

    public List<Expense> GetAll(int userId)
        => _expenseLibrary.GetAll(userId);

    public Expense? GetById(int id, int userId)
        => _expenseLibrary.GetById(id, userId);

    public void Create(ExpenseRequest request, int userId)
    {
        var expense = new Expense
        {
            Title      = request.Title,
            Amount     = request.Amount,
            Date       = request.Date,
            Note       = request.Note,
            Source     = "manual",
            UserId     = userId,
            CategoryId = request.CategoryId
        };
        _expenseLibrary.Create(expense);
    }

    public bool Update(int id, ExpenseRequest request, int userId)
    {
        var expense = new Expense
        {
            Id         = id,
            Title      = request.Title,
            Amount     = request.Amount,
            Date       = request.Date,
            Note       = request.Note,
            UserId     = userId,
            CategoryId = request.CategoryId
        };
        return _expenseLibrary.Update(expense);
    }

    public bool Delete(int id, int userId)
        => _expenseLibrary.Delete(id, userId);
}