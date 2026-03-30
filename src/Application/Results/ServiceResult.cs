using Application.Results.Interfaces;
using Shared.Enums;

namespace Application.Results;

public class ServiceResult : IServiceResult
{
    public bool IsSuccess { get; }
    public ServiceResultType Type { get; }
    public object? Data { get; }
    public string? ErrorMessage { get; }
    public IDictionary<string, string[]>? ValidationErrors { get; }

    public ServiceResult(ServiceResultType type, object? data = null, string? errorMessage = null, IDictionary<string, string[]>? validationErrors = null)
    {
        Type = type;
        Data = data;
        ErrorMessage = errorMessage;
        IsSuccess = type < ServiceResultType.BadRequest;
        ValidationErrors = validationErrors;
    }
}
