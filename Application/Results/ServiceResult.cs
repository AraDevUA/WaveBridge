using Application.Results.Interfaces;
using Domain.Enums;

namespace Application.Results;

public class ServiceResult : IServiceResult
{
    public bool IsSuccess { get; }
    public ServiceResultType Type { get; }
    public object? Data { get; }
    public string? ErrorMessage { get; }

    public ServiceResult(ServiceResultType type, object? data = null, string? errorMessage = null)
    {
        Type = type;
        Data = data;
        ErrorMessage = errorMessage;
        IsSuccess = type < ServiceResultType.BadRequest;
    }
}
