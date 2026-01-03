namespace Domain.Enums;

public enum ServiceResultType
{
    Ok,
    Created,
    NoContent,
    BadRequest,
    Unauthorized,
    Forbidden,
    NotFound,
    Conflict,
    ExternalError,
    Timeout,
    ValidationFailed,
    CriticalError
}
