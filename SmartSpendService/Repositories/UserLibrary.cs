using Microsoft.Data.Sqlite;
using SmartSpendService.Data;
using SmartSpendService.Models;

namespace SmartSpendService.Repositories;

public class UserLibrary
{
    private readonly DatabaseHelper _db;

    public UserLibrary(DatabaseHelper db)
    {
        _db = db;
    }

    public User? GetByEmail(string email)
    {
        using var connection = _db.GetConnection();
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT * FROM Users WHERE Email = @Email;";
        cmd.Parameters.AddWithValue("@Email", email);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return new User
            {
                Id           = reader.GetInt32(0),
                Username     = reader.GetString(1),
                Email        = reader.GetString(2),
                PasswordHash = reader.GetString(3),
                CreatedAt    = DateTime.Parse(reader.GetString(4))
            };
        }

        return null;
    }

    public User? GetByUsername(string username)
    {
        using var connection = _db.GetConnection();
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT * FROM Users WHERE Username = @Username;";
        cmd.Parameters.AddWithValue("@Username", username);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return new User
            {
                Id           = reader.GetInt32(0),
                Username     = reader.GetString(1),
                Email        = reader.GetString(2),
                PasswordHash = reader.GetString(3),
                CreatedAt    = DateTime.Parse(reader.GetString(4))
            };
        }

        return null;
    }

    public void Create(User user)
    {
        using var connection = _db.GetConnection();
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO Users (Username, Email, PasswordHash, CreatedAt)
            VALUES (@Username, @Email, @PasswordHash, @CreatedAt);";

        cmd.Parameters.AddWithValue("@Username",     user.Username);
        cmd.Parameters.AddWithValue("@Email",        user.Email);
        cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
        cmd.Parameters.AddWithValue("@CreatedAt",    user.CreatedAt.ToString("o"));

        cmd.ExecuteNonQuery();
    }
}