namespace Application.Dto.Jwt;
public record JwtOptions
{
    public string Key { get; set; } = default!;
    public string Issuer { get; set; } = default!;
    public string Audience { get; set; } = default!;
    public int ExpiresHours { get; set; }
    public int RefreshTokenLifetimeDays { get; set; }
}
