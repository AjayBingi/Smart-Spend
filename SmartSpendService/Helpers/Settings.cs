namespace SmartSpendService.Helpers;

public class JwtSettings
{
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpiryInDays { get; set; }
}

public class AISettings
{
    public string ApiKey { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
}