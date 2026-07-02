namespace Application.Dto;

public record PageDto<T>
{
    public required IEnumerable<T> Items { get; init; }
    public required int TotalCount { get; init; }
}
