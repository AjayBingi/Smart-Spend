using SmartSpendService.DTOs;
using SmartSpendService.Models;
using SmartSpendService.Repositories;

namespace SmartSpendService.Services;

public class UserProfileService
{
    private readonly UserProfileLibrary _userProfileLibrary;

    public UserProfileService(UserProfileLibrary userProfileLibrary)
    {
        _userProfileLibrary = userProfileLibrary;
    }

    public UserProfile? GetProfile(int userId)
        => _userProfileLibrary.GetByUserId(userId);

    public void SaveProfile(int userId, UserProfileRequest request)
    {
        var profile = new UserProfile
        {
            UserId = userId,
            MonthlyIncome = request.MonthlyIncome,
            RentAmount = request.RentAmount,
            HomeSavingsAmount = request.HomeSavingsAmount
        };

        _userProfileLibrary.Upsert(profile);
    }
}