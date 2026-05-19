namespace SmartSpendService.DTOs;

public class CategorySpending
{
    public string CategoryName { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal Percentage { get; set; }
}

public class MonthlySpending
{
    public string Month { get; set; } = string.Empty;
    public int Year { get; set; }
    public decimal Amount { get; set; }
}

public class BudgetStatus
{
    public string CategoryName { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public decimal Budgeted { get; set; }
    public decimal Spent { get; set; }
    public decimal Remaining { get; set; }
    public bool IsOverBudget { get; set; }
}

public class Anomaly
{
    public string CategoryName { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public decimal Budgeted { get; set; }
    public decimal Spent { get; set; }
    public decimal OverBy { get; set; }
}