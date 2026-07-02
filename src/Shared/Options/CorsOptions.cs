namespace Shared.Options;

public class CorsOptions
{
    public const string SectionName = "Cors";
    public const string PolicyName = "ConfiguredCorsPolicy";

    public string[] AllowedOrigins { get; set; } = [];
    public bool AllowAnyHeader { get; set; } = true;
    public bool AllowAnyMethod { get; set; } = true;
    public bool AllowCredentials { get; set; } = true;
}
