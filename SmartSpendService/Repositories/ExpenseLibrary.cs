using Microsoft.Data.Sqlite;
using SmartSpendService.Data;
using SmartSpendService.Models;

namespace SmartSpendService.Repositories;

public class ExpenseLibrary
{
    private readonly DatabaseHelper _db;

    public ExpenseLibrary(DatabaseHelper db)
    {
        _db = db;
    }

    public List<Expense> GetAll(int userId)
    {
        using var connection = _db.GetConnection();
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            SELECT e.Id, e.Title, e.Amount, e.Date, e.Note, e.Source,
                   e.UserId, e.CategoryId,
                   c.Name, c.Icon, c.Color
            FROM Expenses e
            INNER JOIN Categories c ON e.CategoryId = c.Id
            WHERE e.UserId = @UserId
            ORDER BY e.Date DESC;";
        cmd.Parameters.AddWithValue("@UserId", userId);

        var expenses = new List<Expense>();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            expenses.Add(new Expense
            {
                Id            = reader.GetInt32(0),
                Title         = reader.GetString(1),
                Amount        = reader.GetDecimal(2),
                Date          = DateTime.Parse(reader.GetString(3)),
                Note          = reader.IsDBNull(4) ? null : reader.GetString(4),
                Source        = reader.GetString(5),
                UserId        = reader.GetInt32(6),
                CategoryId    = reader.GetInt32(7),
                CategoryName  = reader.GetString(8),
                CategoryIcon  = reader.GetString(9),
                CategoryColor = reader.GetString(10)
            });
        }

        return expenses;
    }

    public Expense? GetById(int id, int userId)
    {
        using var connection = _db.GetConnection();
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            SELECT e.Id, e.Title, e.Amount, e.Date, e.Note, e.Source,
                   e.UserId, e.CategoryId,
                   c.Name, c.Icon, c.Color
            FROM Expenses e
            INNER JOIN Categories c ON e.CategoryId = c.Id
            WHERE e.Id = @Id AND e.UserId = @UserId;";
        cmd.Parameters.AddWithValue("@Id", id);
        cmd.Parameters.AddWithValue("@UserId", userId);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return new Expense
            {
                Id            = reader.GetInt32(0),
                Title         = reader.GetString(1),
                Amount        = reader.GetDecimal(2),
                Date          = DateTime.Parse(reader.GetString(3)),
                Note          = reader.IsDBNull(4) ? null : reader.GetString(4),
                Source        = reader.GetString(5),
                UserId        = reader.GetInt32(6),
                CategoryId    = reader.GetInt32(7),
                CategoryName  = reader.GetString(8),
                CategoryIcon  = reader.GetString(9),
                CategoryColor = reader.GetString(10)
            };
        }

        return null;
    }

    public void Create(Expense expense)
    {
        using var connection = _db.GetConnection();
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO Expenses (Title, Amount, Date, Note, Source, UserId, CategoryId)
            VALUES (@Title, @Amount, @Date, @Note, @Source, @UserId, @CategoryId);";

        cmd.Parameters.AddWithValue("@Title",      expense.Title);
        cmd.Parameters.AddWithValue("@Amount",     expense.Amount);
        cmd.Parameters.AddWithValue("@Date",       expense.Date.ToString("o"));
        cmd.Parameters.AddWithValue("@Note",       expense.Note ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@Source",     expense.Source);
        cmd.Parameters.AddWithValue("@UserId",     expense.UserId);
        cmd.Parameters.AddWithValue("@CategoryId", expense.CategoryId);

        cmd.ExecuteNonQuery();
    }

    public bool Update(Expense expense)
    {
        using var connection = _db.GetConnection();
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            UPDATE Expenses
            SET Title      = @Title,
                Amount     = @Amount,
                Date       = @Date,
                Note       = @Note,
                CategoryId = @CategoryId
            WHERE Id = @Id AND UserId = @UserId;";

        cmd.Parameters.AddWithValue("@Title",      expense.Title);
        cmd.Parameters.AddWithValue("@Amount",     expense.Amount);
        cmd.Parameters.AddWithValue("@Date",       expense.Date.ToString("o"));
        cmd.Parameters.AddWithValue("@Note",       expense.Note ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@CategoryId", expense.CategoryId);
        cmd.Parameters.AddWithValue("@Id",         expense.Id);
        cmd.Parameters.AddWithValue("@UserId",     expense.UserId);

        return cmd.ExecuteNonQuery() > 0;
    }

    public bool Delete(int id, int userId)
    {
        using var connection = _db.GetConnection();
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = "DELETE FROM Expenses WHERE Id = @Id AND UserId = @UserId;";
        cmd.Parameters.AddWithValue("@Id",     id);
        cmd.Parameters.AddWithValue("@UserId", userId);

        return cmd.ExecuteNonQuery() > 0;
    }
}