namespace SmartSpendService.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class UserProfile
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public decimal MonthlyIncome { get; set; }
    public decimal RentAmount { get; set; }
    public decimal HomeSavingsAmount { get; set; }
}

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
}

public class Expense
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public string? Note { get; set; }
    public string Source { get; set; } = "manual";
    public int UserId { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string CategoryIcon { get; set; } = string.Empty;
    public string CategoryColor { get; set; } = string.Empty;
}

public class Budget
{
    public int Id { get; set; }
    public decimal LimitAmount { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public int UserId { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string CategoryIcon { get; set; } = string.Empty;
}

public class AIInsight
{
    public int Id { get; set; }
    public string InsightText { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public int UserId { get; set; }
}