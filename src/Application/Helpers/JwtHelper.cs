using System.IdentityModel.Tokens.Jwt;

namespace Application.Helpers;

public static class JwtHelper
{
    private static readonly JwtSecurityTokenHandler _handler = new JwtSecurityTokenHandler();
    public static bool IsExpired(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return true;

        try
        {
            var jwtToken = _handler.ReadJwtToken(token);
            return jwtToken.ValidTo < DateTime.UtcNow;
        }
        catch
        {
            return true;
        }
    }
}
