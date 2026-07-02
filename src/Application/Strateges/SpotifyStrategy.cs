using Application.Dto;
using Application.Dto.Requests.Transfers;
using Application.Dto.Streaming;
using Application.Dto.Streaming.Responses.Spotify;
using Application.Helpers.Contracts;
using Application.Strateges.Abstractions;

namespace Application.Strateges;

public class SpotifyStrategy : IStreamingStrategy
{
    private const string BaseUrl = "https://api.spotify.com/v1/";

    private readonly IHttpClientHelper _httpClientHelper;

    public SpotifyStrategy(IHttpClientHelper httpClientHelper)
    {
        _httpClientHelper = httpClientHelper;
    }

    public async Task<PageDto<SourceTrackDto>> GetLikedTracksAsync(LikedTracksPagedRequestDto dto, string accessToken)
    {
        var endpoint = $"{BaseUrl}me/tracks?limit={dto.PageSize}&offset={dto.Page * dto.PageSize}";
        var response = await _httpClientHelper.SendGetRequestAsync<SpotifyLikedTracksResponseDto>(endpoint, accessToken: accessToken, throwOnError: true);

        return new PageDto<SourceTrackDto>
        {
            Items = response.Items
                .Where(x => x.Track is not null)
                .Select(MapTrack)
                .ToList(),
            TotalCount = response.Total
        };
    }

    public async Task<PageDto<SourcePlaylistDto>> GetPlaylistsAsync(PagedRequest dto, string accessToken)
    {
        var endpoint = $"{BaseUrl}me/playlists?limit={dto.PageSize}&offset={dto.Page * dto.PageSize}";
        var response = await _httpClientHelper.SendGetRequestAsync<SpotifyPlaylistListResponseDto>(endpoint, accessToken: accessToken, throwOnError: true);

        return new PageDto<SourcePlaylistDto>
        {
            Items = response.Items.Select(MapPlaylist).ToList(),
            TotalCount = response.Total
        };
    }

    public async Task<IEnumerable<SourceTrackDto>> GetPlaylistTracksAsync(string playlistId, string accessToken)
    {
        var endpoint = $"{BaseUrl}playlists/{playlistId}/tracks";
        var response = await _httpClientHelper.SendGetRequestAsync<SpotifyPlaylistTracksResponseDto>(endpoint, accessToken: accessToken, throwOnError: true);

        return response.Items
            .Where(x => x.Track is not null)
            .Select(MapTrack)
            .ToList();
    }

    public async Task<string?> SearchForTrackAsync(TrackSearchDto track, string accessToken)
    {
        var query = $"track:{track.Name}";
        if (!string.IsNullOrWhiteSpace(track.Artist))
            query += $" artist:{track.Artist}";
        if (!string.IsNullOrWhiteSpace(track.Album))
            query += $" album:{track.Album}";

        var endpoint = $"{BaseUrl}search?q={Uri.EscapeDataString(query)}&type=track&limit=5";
        var response = await _httpClientHelper.SendGetRequestAsync<SpotifySearchResponseDto>(endpoint, accessToken: accessToken, throwOnError: true);

        return response.Tracks.Items.FirstOrDefault(t => !string.IsNullOrWhiteSpace(t.Id))?.Id;
    }

    public async Task AddTrackToPlaylistAsync(string playlistId, string trackId, string accessToken)
    {
        var endpoint = $"{BaseUrl}playlists/{playlistId}/tracks";
        var body = new
        {
            uris = new[] { $"spotify:track:{trackId}" }
        };

        await _httpClientHelper.SendPostJsonRequestAsync<object>(endpoint, body, accessToken: accessToken, throwOnError: true);
    }

    public async Task<SourcePlaylistDto> CreatePlaylistAsync(string name, string accessToken, string? description = null, bool isPublic = false)
    {
        var userResponse = await _httpClientHelper.SendGetRequestAsync<SpotifyUserProfileResponseDto>($"{BaseUrl}me", accessToken: accessToken, throwOnError: true);
        var endpoint = $"{BaseUrl}users/{userResponse.Id}/playlists";
        var body = new
        {
            name,
            description,
            @public = isPublic
        };

        var response = await _httpClientHelper.SendPostJsonRequestAsync<SpotifyPlaylistResponseDto>(endpoint, body, accessToken: accessToken, throwOnError: true);
        return MapPlaylist(response);
    }

    public async Task<SourcePlaylistDto> GetPlaylistInfoAsync(string playlistId, string accessToken)
    {
        var endpoint = $"{BaseUrl}playlists/{playlistId}";
        var response = await _httpClientHelper.SendGetRequestAsync<SpotifyPlaylistResponseDto>(endpoint, accessToken: accessToken, throwOnError: true);
        return MapPlaylist(response);
    }

    private static SourceTrackDto MapTrack(SpotifyPlaylistItemDto item)
    {
        return MapTrack(item.Track!);
    }

    private static SourceTrackDto MapTrack(SpotifyTrackDto track)
    {
        return new SourceTrackDto
        {
            SourceId = track.Id,
            TrackName = track.Name,
            Artist = track.Artists.FirstOrDefault()?.Name ?? string.Empty,
            Album = track.Album?.Name,
            ArtworkUrl = track.Album?.Images.FirstOrDefault()?.Url
        };
    }

    private static SourcePlaylistDto MapPlaylist(SpotifyPlaylistSummaryDto playlist)
    {
        return new SourcePlaylistDto
        {
            SourceId = playlist.Id,
            Name = playlist.Name,
            Description = playlist.Description,
            ArtworkUrl = playlist.Images.FirstOrDefault()?.Url,
            Url = playlist.ExternalUrls?.Spotify,
            TrackCount = playlist.Tracks?.Total
        };
    }

    private static SourcePlaylistDto MapPlaylist(SpotifyPlaylistResponseDto playlist)
    {
        return new SourcePlaylistDto
        {
            SourceId = playlist.Id,
            Name = playlist.Name,
            Description = playlist.Description,
            ArtworkUrl = playlist.Images.FirstOrDefault()?.Url,
            Url = playlist.ExternalUrls?.Spotify,
            TrackCount = playlist.Tracks?.Total
        };
    }
}
