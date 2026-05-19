using Microsoft.Data.Sqlite;
using SmartSpendService.Data;
using SmartSpendService.Models;

namespace SmartSpendService.Repositories;

public class UserProfileLibrary
{
    private readonly DatabaseHelper _db;

    public UserProfileLibrary(DatabaseHelper db)
    {
        _db = db;
    }

    public UserProfile? GetByUserId(int userId)
    {
        using var connection = _db.GetConnection();
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            SELECT Id, UserId, MonthlyIncome, RentAmount, HomeSavingsAmount
            FROM UserProfiles
            WHERE UserId = @UserId;";
        cmd.Parameters.AddWithValue("@UserId", userId);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return new UserProfile
            {
                Id                = reader.GetInt32(0),
                UserId            = reader.GetInt32(1),
                MonthlyIncome     = reader.GetDecimal(2),
                RentAmount        = reader.GetDecimal(3),
                HomeSavingsAmount = reader.GetDecimal(4)
            };
        }

        return null;
    }

    public void Upsert(UserProfile profile)
    {
        using var connection = _db.GetConnection();
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO UserProfiles (UserId, MonthlyIncome, RentAmount, HomeSavingsAmount)
            VALUES (@UserId, @MonthlyIncome, @RentAmount, @HomeSavingsAmount)
            ON CONFLICT(UserId) DO UPDATE SET
                MonthlyIncome     = @MonthlyIncome,
                RentAmount        = @RentAmount,
                HomeSavingsAmount = @HomeSavingsAmount;";

        cmd.Parameters.AddWithValue("@UserId",            profile.UserId);
        cmd.Parameters.AddWithValue("@MonthlyIncome",     profile.MonthlyIncome);
        cmd.Parameters.AddWithValue("@RentAmount",        profile.RentAmount);
        cmd.Parameters.AddWithValue("@HomeSavingsAmount", profile.HomeSavingsAmount);

        cmd.ExecuteNonQuery();
    }
}