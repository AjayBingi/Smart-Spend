namespace SmartSpendService.DTOs;

public class ParseExpenseRequest
{
    public string Text { get; set; } = string.Empty;
}

public class ParseExpenseResponse
{
    public string Title { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string? Note { get; set; }
}

public class MonthlyReportRequest
{
    public int Month { get; set; }
    public int Year { get; set; }
}

public class AITextResponse
{
    public string Result { get; set; } = string.Empty;
}

public class InvestmentRequest
{
    public int Month { get; set; }
    public int Year { get; set; }
}
