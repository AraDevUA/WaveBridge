using Application.Providers.Contracts;
using Microsoft.AspNetCore.DataProtection;

namespace Application.Providers;

public class OAuthStateProvider : IOAuthStateProvider
{
    private readonly IDataProtector _protector;

    public OAuthStateProvider(IDataProtectionProvider provider)
    {
        _protector = provider.CreateProtector("oauth-state");
    }

    public string Protect(Guid userId)
        => _protector.Protect(userId.ToString());

    public Guid Unprotect(string state)
        => Guid.Parse(_protector.Unprotect(state));
}
