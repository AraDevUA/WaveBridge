using Shared.Enums;

namespace Application.Dto.Requests.Transfers;

public record CreateTransferRequestDto
{
    public StreamingService Source { get; init; }
    public bool IsPublic { get; init; }
    public bool ToSinglePlaylist { get; init; }
    public ICollection<CreateTransferPlaylistRequestDto> Playlists { get; init; } = [];
}
