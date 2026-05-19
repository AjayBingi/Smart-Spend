using SmartSpendService.DTOs;
using SmartSpendService.Models;
using SmartSpendService.Repositories;

namespace SmartSpendService.Services;

public class BudgetService
{
    private readonly BudgetLibrary _budgetLibrary;

    public BudgetService(BudgetLibrary budgetLibrary)
    {
        _budgetLibrary = budgetLibrary;
    }

    public List<Budget> GetAll(int userId)
        => _budgetLibrary.GetAll(userId);

    public Budget? GetById(int id, int userId)
        => _budgetLibrary.GetById(id, userId);

    public void Create(BudgetRequest request, int userId)
    {
        var budget = new Budget
        {
            LimitAmount = request.LimitAmount,
            Month       = request.Month,
            Year        = request.Year,
            UserId      = userId,
            CategoryId  = request.CategoryId
        };
        _budgetLibrary.Create(budget);
    }

    public bool Update(int id, BudgetRequest request, int userId)
    {
        var budget = new Budget
        {
            Id          = id,
            LimitAmount = request.LimitAmount,
            Month       = request.Month,
            Year        = request.Year,
            UserId      = userId,
            CategoryId  = request.CategoryId
        };
        return _budgetLibrary.Update(budget);
    }

    public bool Delete(int id, int userId)
        => _budgetLibrary.Delete(id, userId);
}