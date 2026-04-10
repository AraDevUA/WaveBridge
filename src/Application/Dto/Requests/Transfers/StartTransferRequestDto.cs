using Shared.Enums;

namespace Application.Dto.Requests.Transfers;

public record StartTransferRequestDto
{
    public StreamingService Source { get; init; }
    public StreamingService Destination { get; init; }
    public bool IsPublic { get; init; }
    public bool ToSinglePlaylist { get; init; }
    public ICollection<CreateTransferPlaylistRequestDto> Playlists { get; init; } = [];
}
