namespace Application.Services;

internal sealed record TransferExecutionResult(bool IsSuccess, string? ErrorMessage)
{
    public static TransferExecutionResult Success()
    {
        return new TransferExecutionResult(true, null);
    }

    public static TransferExecutionResult Failure(string errorMessage)
    {
        return new TransferExecutionResult(false, errorMessage);
    }
}
