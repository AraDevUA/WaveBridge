namespace Application.Providers.Contracts;

public interface IOAuthStateProvider
{
    string Protect(Guid userId);
    Guid Unprotect(string state);
}
