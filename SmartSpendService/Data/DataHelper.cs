using Microsoft.Data.Sqlite;

namespace SmartSpendService.Data;

public class DatabaseHelper
{
    private readonly string _connectionString;
    private readonly string _sqlFilePath;

    public DatabaseHelper(string connectionString, string sqlFilePath)
    {
        _connectionString = connectionString;
        _sqlFilePath = sqlFilePath;
    }

    public SqliteConnection GetConnection()
    {
        return new SqliteConnection(_connectionString);
    }

    public void Initialize()
    {
        var sql = File.ReadAllText(_sqlFilePath);

        using var connection = GetConnection();
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = sql;
        cmd.ExecuteNonQuery();
    }
}