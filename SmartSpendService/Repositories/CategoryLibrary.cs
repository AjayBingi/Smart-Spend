using Microsoft.Data.Sqlite;
using SmartSpendService.Data;
using SmartSpendService.Models;

namespace SmartSpendService.Repositories;

public class CategoryLibrary
{
    private readonly DatabaseHelper _db;

    public CategoryLibrary(DatabaseHelper db)
    {
        _db = db;
    }

    public List<Category> GetAll()
    {
        using var connection = _db.GetConnection();
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT Id, Name, Icon, Color FROM Categories ORDER BY Name;";

        var categories = new List<Category>();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            categories.Add(new Category
            {
                Id    = reader.GetInt32(0),
                Name  = reader.GetString(1),
                Icon  = reader.GetString(2),
                Color = reader.GetString(3)
            });
        }

        return categories;
    }

    public Category? GetById(int id)
    {
        using var connection = _db.GetConnection();
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT Id, Name, Icon, Color FROM Categories WHERE Id = @Id;";
        cmd.Parameters.AddWithValue("@Id", id);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return new Category
            {
                Id    = reader.GetInt32(0),
                Name  = reader.GetString(1),
                Icon  = reader.GetString(2),
                Color = reader.GetString(3)
            };
        }

        return null;
    }
}