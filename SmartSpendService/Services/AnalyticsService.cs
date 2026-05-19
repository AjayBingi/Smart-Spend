using SmartSpendService.DTOs;
using SmartSpendService.Repositories;

namespace SmartSpendService.Services;

public class AnalyticsService
{
    private readonly ExpenseLibrary _expenseLibrary;
    private readonly BudgetLibrary _budgetLibrary;

    public AnalyticsService(ExpenseLibrary expenseLibrary, BudgetLibrary budgetLibrary)
    {
        _expenseLibrary = expenseLibrary;
        _budgetLibrary  = budgetLibrary;
    }

    public List<CategorySpending> GetByCategory(int userId, int month, int year)
    {
        var expenses = _expenseLibrary.GetAll(userId)
            .Where(e => e.Date.Month == month && e.Date.Year == year)
            .ToList();

        var total = expenses.Sum(e => e.Amount);
        if (total == 0) return new List<CategorySpending>();

        return expenses
            .GroupBy(e => new { e.CategoryName, e.CategoryColor, e.CategoryIcon })
            .Select(g => new CategorySpending
            {
                CategoryName = g.Key.CategoryName,
                Color        = g.Key.CategoryColor,
                Icon         = g.Key.CategoryIcon,
                Amount       = g.Sum(e => e.Amount),
                Percentage   = Math.Round(g.Sum(e => e.Amount) / total * 100, 2)
            })
            .OrderByDescending(c => c.Amount)
            .ToList();
    }

    public List<MonthlySpending> GetMonthlyTrend(int userId, int month, int year)
    {
        return _expenseLibrary.GetAll(userId)
            .Where(e => e.Date.Month <= month && e.Date.Year == year)
            .GroupBy(e => new { e.Date.Month, e.Date.Year })
            .Select(g => new MonthlySpending
            {
                Month  = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM yyyy"),
                Year   = g.Key.Year,
                Amount = g.Sum(e => e.Amount)
            })
            .OrderBy(m => m.Month)
            .ToList();
    }

    public List<BudgetStatus> GetBudgetStatus(int userId, int month, int year)
    {
        var budgets = _budgetLibrary.GetAll(userId)
            .Where(b => b.Month == month && b.Year == year)
            .ToList();

        var expenses = _expenseLibrary.GetAll(userId)
            .Where(e => e.Date.Month == month && e.Date.Year == year)
            .ToList();

        return budgets.Select(b =>
        {
            var spent = expenses
                .Where(e => e.CategoryId == b.CategoryId)
                .Sum(e => e.Amount);

            return new BudgetStatus
            {
                CategoryName = b.CategoryName,
                Icon         = b.CategoryIcon,
                Budgeted     = b.LimitAmount,
                Spent        = spent,
                Remaining    = b.LimitAmount - spent,
                IsOverBudget = spent > b.LimitAmount
            };
        })
        .OrderByDescending(b => b.Spent)
        .ToList();
    }

    public List<Anomaly> GetAnomalies(int userId, int month, int year)
    {
        var budgetStatuses = GetBudgetStatus(userId, month, year);

        return budgetStatuses
            .Where(b => b.IsOverBudget)
            .Select(b => new Anomaly
            {
                CategoryName = b.CategoryName,
                Icon         = b.Icon,
                Budgeted     = b.Budgeted,
                Spent        = b.Spent,
                OverBy       = b.Spent - b.Budgeted
            })
            .OrderByDescending(a => a.OverBy)
            .ToList();
    }
}