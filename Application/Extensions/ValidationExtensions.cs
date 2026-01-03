using Application.Localization;
using Microsoft.AspNetCore.Identity;
using System.Globalization;

namespace Application.Extensions;

public static class ValidationExtensions
{
    public static IDictionary<string, string[]> ToValidationErrors(this IEnumerable<IdentityError> errors)
    {
        if (errors == null) return new Dictionary<string, string[]>();

        return errors
            .GroupBy(e => e.Code)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.Description).ToArray()
            );
    }
    public static IDictionary<string, string[]> ToValidationErrors(this IDictionary<string, List<string>> errors)
    {
        if (errors == null) return new Dictionary<string, string[]>();
        return errors.ToDictionary(
               g => g.Key,
               g => g.Value?.ToArray() ?? Array.Empty<string>()
            );
    }
    public static IDictionary<string, string[]> ToValidationErrors(string key, string message)
    {
        var localizedMessage = SystemMessages.ResourceManager
        .GetString(message, CultureInfo.CurrentCulture) ?? message;

        return new Dictionary<string, string[]>
            {
                { key, new[] { localizedMessage } }
            };
    }
}
