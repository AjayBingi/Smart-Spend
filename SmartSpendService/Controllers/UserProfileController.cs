using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartSpendService.DTOs;
using SmartSpendService.Helpers;
using SmartSpendService.Services;

namespace SmartSpendService.Controllers;

[ApiController]
[Route("api/profile")]
[Authorize]
public class UserProfileController : ControllerBase
{
    private readonly UserProfileService _userProfileService;

    public UserProfileController(UserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }

    [HttpGet]
    public IActionResult GetProfile()
    {
        var userId = ClaimsHelper.GetUserId(User);
        var profile = _userProfileService.GetProfile(userId);
        if (profile == null)
            return NotFound(new { message = "Profile not found. Please set up your profile." });
        return Ok(profile);
    }

    [HttpPost]
    public IActionResult SaveProfile([FromBody] UserProfileRequest request)
    {
        if (request.MonthlyIncome <= 0)
            return BadRequest(new { message = "Valid monthly income is required." });

        var userId = ClaimsHelper.GetUserId(User);
        _userProfileService.SaveProfile(userId, request);
        return Ok(new { message = "Profile saved successfully." });
    }
}