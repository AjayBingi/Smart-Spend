namespace SmartSpendService.DTOs;

public class ExpenseRequest
{
    public string Title { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public string? Note { get; set; }
    public int CategoryId { get; set; }
}

public class BudgetRequest
{
    public decimal LimitAmount { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public int CategoryId { get; set; }
}

public class UserProfileRequest
{
    public decimal MonthlyIncome { get; set; }
    public decimal RentAmount { get; set; }
    public decimal HomeSavingsAmount { get; set; }
}