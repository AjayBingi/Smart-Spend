using System.Security.Claims;

namespace SmartSpendService.Helpers;

public static class ClaimsHelper
{
    public static int GetUserId(ClaimsPrincipal user)
    {
        var claim = user.FindFirst(ClaimTypes.NameIdentifier);
        if (claim == null) throw new UnauthorizedAccessException("User ID not found in token.");
        return int.Parse(claim.Value);
    }
}