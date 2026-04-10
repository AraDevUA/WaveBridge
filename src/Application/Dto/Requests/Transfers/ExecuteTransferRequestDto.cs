using Shared.Enums;

namespace Application.Dto.Requests.Transfers;

public record ExecuteTransferRequestDto
{
    public StreamingService Destination { get; init; }
    public bool? IsPublic { get; init; }
    public bool? ToSinglePlaylist { get; init; }
}
