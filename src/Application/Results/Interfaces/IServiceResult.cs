using Domain.Enums;

namespace Application.Results.Interfaces;

public interface IServiceResult
{
    bool IsSuccess { get; }
    ServiceResultType Type { get; }
    object? Data { get; }
    string? ErrorMessage { get; }
    IDictionary<string, string[]>? ValidationErrors { get; }
}

