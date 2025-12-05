using System.Text.Json.Serialization;

namespace Application.Dto.Response.Auth;

public record GoogleTokenResponseDto
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; init; }

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; init; }

    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; init; }

    [JsonPropertyName("scope")]
    public string Scope { get; init; }

    [JsonPropertyName("token_type")]
    public string TokenType { get; init; }

    [JsonPropertyName("id_token")]
    public string IdToken { get; init; }
}
