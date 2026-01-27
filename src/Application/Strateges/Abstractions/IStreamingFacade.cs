using Application.Dto;
using Application.Dto.Requests.Transfers;
using Application.Dto.Streaming;
using Domain.Entities;
using Domain.Enums;

namespace Application.Strateges.Abstractions;

public interface IStreamingFacade
{
    Task<string> EnsureValidAccessTokenAsync(StreamingService platform, UserStreamingConnection connection);
    Task AddTrackToPlaylistAsync(StreamingService platform, string playlistId, string trackId, string accessToken);
    Task<PageDto<SourceTrackDto>> GetLikedTracksAsync(StreamingService platform, LikedTracksPagedRequestDto pagedRequest, string accessToken);
    Task<SourcePlaylistDto> GetPlaylistInfoAsync(StreamingService platform, string playlistId, string accessToken);
    Task<IEnumerable<SourceTrackDto>> GetPlaylistTracksAsync(StreamingService platform, string playlistId, string accessToken);
    Task<string?> SearchForTrackAsync(StreamingService platform, TrackSearchDto track, string accessToken);
    Task<string> CreatePlaylistAsync(StreamingService platform, string name, string accessToken, string? description = null, bool isPublic = false);

}
