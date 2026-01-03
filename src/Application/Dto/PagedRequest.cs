namespace Application.Dto;

public abstract record PagedRequest
{
    public int Page { get; init; }
    public int PageSize { get; init; } = 20;
}
