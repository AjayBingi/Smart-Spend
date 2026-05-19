using Microsoft.Data.Sqlite;
using SmartSpendService.Data;
using SmartSpendService.Models;

namespace SmartSpendService.Repositories;

public class BudgetLibrary
{
    private readonly DatabaseHelper _db;

    public BudgetLibrary(DatabaseHelper db)
    {
        _db = db;
    }

    public List<Budget> GetAll(int userId)
    {
        using var connection = _db.GetConnection();
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            SELECT b.Id, b.LimitAmount, b.Month, b.Year,
                   b.UserId, b.CategoryId,
                   c.Name, c.Icon
            FROM Budgets b
            INNER JOIN Categories c ON b.CategoryId = c.Id
            WHERE b.UserId = @UserId
            ORDER BY b.Year DESC, b.Month DESC;";
        cmd.Parameters.AddWithValue("@UserId", userId);

        var budgets = new List<Budget>();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            budgets.Add(new Budget
            {
                Id           = reader.GetInt32(0),
                LimitAmount  = reader.GetDecimal(1),
                Month        = reader.GetInt32(2),
                Year         = reader.GetInt32(3),
                UserId       = reader.GetInt32(4),
                CategoryId   = reader.GetInt32(5),
                CategoryName = reader.GetString(6),
                CategoryIcon = reader.GetString(7)
            });
        }

        return budgets;
    }

    public Budget? GetById(int id, int userId)
    {
        using var connection = _db.GetConnection();
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            SELECT b.Id, b.LimitAmount, b.Month, b.Year,
                   b.UserId, b.CategoryId,
                   c.Name, c.Icon
            FROM Budgets b
            INNER JOIN Categories c ON b.CategoryId = c.Id
            WHERE b.Id = @Id AND b.UserId = @UserId;";
        cmd.Parameters.AddWithValue("@Id",     id);
        cmd.Parameters.AddWithValue("@UserId", userId);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return new Budget
            {
                Id           = reader.GetInt32(0),
                LimitAmount  = reader.GetDecimal(1),
                Month        = reader.GetInt32(2),
                Year         = reader.GetInt32(3),
                UserId       = reader.GetInt32(4),
                CategoryId   = reader.GetInt32(5),
                CategoryName = reader.GetString(6),
                CategoryIcon = reader.GetString(7)
            };
        }

        return null;
    }

    public void Create(Budget budget)
    {
        using var connection = _db.GetConnection();
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO Budgets (LimitAmount, Month, Year, UserId, CategoryId)
            VALUES (@LimitAmount, @Month, @Year, @UserId, @CategoryId);";

        cmd.Parameters.AddWithValue("@LimitAmount", budget.LimitAmount);
        cmd.Parameters.AddWithValue("@Month",       budget.Month);
        cmd.Parameters.AddWithValue("@Year",        budget.Year);
        cmd.Parameters.AddWithValue("@UserId",      budget.UserId);
        cmd.Parameters.AddWithValue("@CategoryId",  budget.CategoryId);

        cmd.ExecuteNonQuery();
    }

    public bool Update(Budget budget)
    {
        using var connection = _db.GetConnection();
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            UPDATE Budgets
            SET LimitAmount = @LimitAmount,
                Month       = @Month,
                Year        = @Year,
                CategoryId  = @CategoryId
            WHERE Id = @Id AND UserId = @UserId;";

        cmd.Parameters.AddWithValue("@LimitAmount", budget.LimitAmount);
        cmd.Parameters.AddWithValue("@Month",       budget.Month);
        cmd.Parameters.AddWithValue("@Year",        budget.Year);
        cmd.Parameters.AddWithValue("@CategoryId",  budget.CategoryId);
        cmd.Parameters.AddWithValue("@Id",          budget.Id);
        cmd.Parameters.AddWithValue("@UserId",      budget.UserId);

        return cmd.ExecuteNonQuery() > 0;
    }

    public bool Delete(int id, int userId)
    {
        using var connection = _db.GetConnection();
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = "DELETE FROM Budgets WHERE Id = @Id AND UserId = @UserId;";
        cmd.Parameters.AddWithValue("@Id",     id);
        cmd.Parameters.AddWithValue("@UserId", userId);

        return cmd.ExecuteNonQuery() > 0;
    }
}