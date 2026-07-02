namespace Application.Dto.Requests.Transfers;
public record LikedTracksPagedRequestDto : PagedRequest
{
    public string? PageToken { get; init; }
}