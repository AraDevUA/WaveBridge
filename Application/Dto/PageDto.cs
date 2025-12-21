namespace Application.Dto;

public sealed record PageDto<T>
{
    public required IEnumerable<T> Items { get; init; }
    public required int TotalCount { get; init; }
}