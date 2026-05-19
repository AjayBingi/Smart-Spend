using System.Text.Json;
using System.Text.RegularExpressions;
using SmartSpendService.DTOs;
using SmartSpendService.Repositories;

namespace SmartSpendService.Services;

public class AIService
{
    private readonly AILibrary _aiLibrary;
    private readonly ExpenseLibrary _expenseLibrary;
    private readonly BudgetLibrary _budgetLibrary;
    private readonly CategoryLibrary _categoryLibrary;
    private readonly UserProfileLibrary _userProfileLibrary;

    public AIService(
        AILibrary aiLibrary,
        ExpenseLibrary expenseLibrary,
        BudgetLibrary budgetLibrary,
        CategoryLibrary categoryLibrary,
        UserProfileLibrary userProfileLibrary)
    {
        _aiLibrary = aiLibrary;
        _expenseLibrary = expenseLibrary;
        _budgetLibrary = budgetLibrary;
        _categoryLibrary = categoryLibrary;
        _userProfileLibrary = userProfileLibrary;
    }

    public async Task<ParseExpenseResponse> ParseExpense(string text)
    {
        var categories = _categoryLibrary.GetAll();
        var categoryList = string.Join(", ", categories.Select(c => c.Name));

        var prompt = $@"
            You are a personal finance assistant. Parse the following expense text and return ONLY a JSON object with no extra text, no markdown, no backticks.

            Expense text: ""{text}""

            Available categories: {categoryList}

            Return exactly this JSON format:
            {{
            ""title"": ""short expense title"",
            ""amount"": 000.00,
            ""categoryName"": ""one of the available categories"",
            ""note"": ""optional extra detail or null""
            }}";

        var result = await _aiLibrary.AskAI(prompt);

        // Clean response
        var clean = Regex.Replace(result, @"```json|```", "").Trim();

        var parsed = JsonSerializer.Deserialize<ParseExpenseResponse>(clean, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return parsed ?? new ParseExpenseResponse { Title = text, Amount = 0 };
    }

    public async Task<string> MonthlyReport(int userId, int month, int year)
    {
        var budgets = _budgetLibrary.GetAll(userId)
            .Where(b => b.Month == month && b.Year == year)
            .ToList();

        if (!budgets.Any())
            return "No budgets found for this month. Please set your monthly budgets first.";

        var totalBudgeted = budgets.Sum(b => b.LimitAmount);

        var expenses = _expenseLibrary.GetAll(userId)
            .Where(e => e.Date.Month == month && e.Date.Year == year)
            .ToList();

        var totalSpent = expenses.Sum(e => e.Amount);

        var byCategory = expenses
            .GroupBy(e => e.CategoryName)
            .Select(g => $"{g.Key}: ₹{g.Sum(e => e.Amount):F2}")
            .ToList();

        var budgetSummary = string.Join(", ", budgets.Select(b => $"{b.CategoryName} budget ₹{b.LimitAmount:F2}"));

        var prompt = $@"
            You are a personal finance assistant for an Indian user.
            Analyze this monthly spending and give a helpful, friendly report in 4-5 sentences.

            Month: {month}/{year}
            Total budgeted: ₹{totalBudgeted:F2}
            Total spent: ₹{totalSpent:F2}
            Spending by category: {string.Join(", ", byCategory)}
            Budgets: {budgetSummary}

            Give insights on:
            - Overall spending summary
            - Which category had highest spend
            - Whether they stayed within budgets
            - One practical tip to save money next month
            Keep it concise and friendly.";

        return await _aiLibrary.AskAI(prompt);
    }

    public async Task<string> SpendingTips(int userId)
    {
        var expenses = _expenseLibrary.GetAll(userId)
            .Where(e => e.Date >= DateTime.UtcNow.AddMonths(-1))
            .ToList();

        if (!expenses.Any())
            return "No recent expenses found. Start adding expenses to get personalized tips.";

        var total = expenses.Sum(e => e.Amount);
        var byCategory = expenses
            .GroupBy(e => e.CategoryName)
            .Select(g => $"{g.Key}: ₹{g.Sum(e => e.Amount):F2}")
            .ToList();

        var prompt = $@"
            You are a personal finance assistant for an Indian user living in Hyderabad.
            Based on their last 30 days spending, give 3 practical money-saving tips.

            Total spent: ₹{total:F2}
            Spending by category: {string.Join(", ", byCategory)}

            Give exactly 3 short, practical, India-specific tips.
            Format as a numbered list. Keep each tip under 2 sentences.";

        return await _aiLibrary.AskAI(prompt);
    }

    public async Task<string> InvestmentAdvice(int userId, int month, int year)
    {
        var profile = _userProfileLibrary.GetByUserId(userId);
        if (profile == null)
            throw new Exception("User profile not found. Please set your monthly income, rent, and home savings first.");

        var budgets = _budgetLibrary.GetAll(userId)
            .Where(b => b.Month == month && b.Year == year)
            .ToList();

        if (!budgets.Any())
            throw new Exception("No budgets found for this month. Please set your monthly budgets first.");

        var totalBudgeted = budgets.Sum(b => b.LimitAmount);
        var disposableIncome = profile.MonthlyIncome - profile.RentAmount - profile.HomeSavingsAmount;
        var surplus = disposableIncome - totalBudgeted;

        var byCategory = budgets
            .Select(b => $"{b.CategoryName}: ₹{b.LimitAmount:F2}")
            .ToList();

        var prompt = $@"
            You are a personal finance and investment advisor for an Indian user living in Hyderabad.
            Based on their monthly finances, suggest a practical SIP investment plan.

            Monthly income: ₹{profile.MonthlyIncome:F2}
            Rent paid: ₹{profile.RentAmount:F2}
            Amount sent home (family savings): ₹{profile.HomeSavingsAmount:F2}
            Disposable income (after rent and home savings): ₹{disposableIncome:F2}
            Total budgeted expenses this month: ₹{totalBudgeted:F2}
            Remaining surplus for investment: ₹{surplus:F2}
            Budget by category: {string.Join(", ", byCategory)}

            Give advice on:
            - How much they should invest in SIPs based on their surplus
            - Specific SIP fund categories to consider (large cap, mid cap, ELSS, index funds, etc.)
            - A suggested monthly SIP allocation breakdown in rupees
            - One category where they can reduce budget to increase SIP amount
            - Whether their home savings and rent are reasonable relative to income

            Keep advice practical, India-specific, and friendly. Format clearly with rupee amounts.
            Reminder: This is for educational purposes only, not formal financial advice.";

        return await _aiLibrary.AskAI(prompt);
    }
}