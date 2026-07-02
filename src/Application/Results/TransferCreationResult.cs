using Domain.Entities;

namespace Application.Results;

internal record TransferCreationResult(TransferOperation? TransferOperation, string? ErrorMessage)
{
    public bool IsSuccess => TransferOperation is not null;

    public static TransferCreationResult Success(TransferOperation transferOperation)
    {
        return new TransferCreationResult(transferOperation, null);
    }

    public static TransferCreationResult Failure(string errorMessage)
    {
        return new TransferCreationResult(null, errorMessage);
    }
}
