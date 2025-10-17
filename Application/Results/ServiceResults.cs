using Application.Results.Interfaces;
using Domain.Enums;

namespace Application.Results;

public class ServiceResults
{
    public static IServiceResult Ok(object? data) => new ServiceResult(ServiceResultType.Ok, data);
    public static IServiceResult Created(object? data) => new ServiceResult(ServiceResultType.Created, data);
    public static IServiceResult NoContent() => new ServiceResult(ServiceResultType.NoContent);
    public static IServiceResult Failed(string errorMessage) => new ServiceResult(ServiceResultType.Conflict, errorMessage);
    public static IServiceResult BadRequest(string errorMessage) => new ServiceResult(ServiceResultType.BadRequest, errorMessage);
    public static IServiceResult Unauthorized() => new ServiceResult(ServiceResultType.Unauthorized);
    public static IServiceResult NotFound() => new ServiceResult(ServiceResultType.NotFound);

}
