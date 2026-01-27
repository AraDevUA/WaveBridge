using Domain.Enums;

namespace Application.Dto.Request.Transfers;

public record StartPlaylistTransferRequestDto
{
    public StreamingService Source { get; init; }
    public StreamingService Destination { get; init; }
    public string SourcePlaylistId { get; init; }
    public bool isPublic { get; init; } = false;
}
