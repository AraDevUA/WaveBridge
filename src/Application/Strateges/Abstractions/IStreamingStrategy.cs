using Application.Dto;
using Application.Dto.Requests.Transfers;
using Application.Dto.Streaming;

namespace Application.Strateges.Abstractions;

public interface IStreamingStrategy
{
    Task<PageDto<SourceTrackDto>> GetLikedTracksAsync(LikedTracksPagedRequestDto dto, string accessToken);
    Task<PageDto<SourcePlaylistDto>> GetPlaylistsAsync(PagedRequest dto, string accessToken);
    Task<IEnumerable<SourceTrackDto>> GetPlaylistTracksAsync(string playlistId, string accessToken);
    Task<SourcePlaylistDto> GetPlaylistInfoAsync(string playlistId, string accessToken);
    Task<string?> SearchForTrackAsync(TrackSearchDto track, string accessToken);
    Task AddTrackToPlaylistAsync(string playlistId, string trackId, string accessToken);
    Task<SourcePlaylistDto> CreatePlaylistAsync(string name, string accessToken, string? description = null, bool isPublic = false);
}
