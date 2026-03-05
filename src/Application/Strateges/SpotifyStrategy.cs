using Application.Dto;
using Application.Dto.Requests.Transfers;
using Application.Dto.Streaming;
using Application.Dto.Streaming.Responses.Spotify;
using Application.Helpers;
using Application.Helpers.Contracts;
using Application.Strateges.Abstractions;
using System.Text.RegularExpressions;

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
        var items = response.Items
            .Where(x => x.Track is not null)
            .Select(x => new SourceTrackDto()
            {
                SourceId = x.Track.Id,
                TrackName = x.Track.Name,
                Artist = x.Track.Artists.FirstOrDefault()?.Name,
                Album = x.Track.Album.Name
            }).ToList();

        return new PageDto<SourceTrackDto>
        {
            Items = items,
            TotalCount = response.Total
        };
    }
    public async Task<IEnumerable<SourceTrackDto>> GetPlaylistTracksAsync(string playlistId, string accessToken)
    {
        var endpoint = $"{BaseUrl}playlists/{playlistId}/tracks";

        var response = await _httpClientHelper.SendGetRequestAsync<SpotifyPlaylistTracksResponseDto>(endpoint, accessToken: accessToken, throwOnError: true);

        return response.Items
            .Where(x => x.Track is not null)
            .Select(x => new SourceTrackDto()
            {
                SourceId = x.Track.Id,
                TrackName = x.Track.Name,
                Artist = x.Track.Artists.FirstOrDefault()?.Name,
                Album = x.Track.Album.Name
            }).ToList();
    }
    public async Task<string?> SearchForTrackAsync(TrackSearchDto track, string accessToken)
    {
        //TODO: Improve search query construction, maybe use Spotify's advanced search syntax more effectivelyp
        var query = $"track:{track.Name}";
        if (!string.IsNullOrWhiteSpace(track.Artist))
            query += $" artist:{track.Artist}";
        if (!string.IsNullOrWhiteSpace(track.Album))
            query += $" album:{track.Album}";

        var endpoint = $"{BaseUrl}search?q={Uri.EscapeDataString(query)}&type=track&limit=5";

        var response = await _httpClientHelper.SendGetRequestAsync<SpotifySearchResponseDto>(
            endpoint, accessToken: accessToken, throwOnError: true);

        var resultTrack = response.Tracks.Items.FirstOrDefault(t => !string.IsNullOrWhiteSpace(t.Id));
        return resultTrack?.Id;
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
    public async Task<string> CreatePlaylistAsync(string name, string accessToken, string? description = null, bool isPublic = false)
    {
        var userEndpoint = $"{BaseUrl}me";
        var userResponse = await _httpClientHelper.SendGetRequestAsync<SpotifyUserProfileResponseDto>(userEndpoint, accessToken: accessToken, throwOnError: true);

        var createPlaylistEndpoint = $"{BaseUrl}users/{userResponse.Id}/playlists";
        var body = new
        {
            name = name,
            description = description,
            @public = isPublic
        };

        var playlistResponse = await _httpClientHelper.SendPostJsonRequestAsync<SpotifyCreatePlaylistResponseDto>(createPlaylistEndpoint, body, accessToken: accessToken, throwOnError: true);
        return playlistResponse.Id;
    }
    public async Task<SourcePlaylistDto> GetPlaylistInfoAsync(string playlistId, string accessToken)
    {
        var endpoint = $"{BaseUrl}playlists/{playlistId}";

        var response = await _httpClientHelper.SendGetRequestAsync<SpotifyPlaylistResponseDto>(
            endpoint,
            accessToken: accessToken,
            throwOnError: true);

        return new SourcePlaylistDto
        {
            Name = response.Name,
            Description = response.Description
        };
    }
    
}
