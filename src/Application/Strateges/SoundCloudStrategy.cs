using Application.Dto;
using Application.Dto.Options.Auth.SoundCloud;
using Application.Dto.Requests.Transfers;
using Application.Dto.Streaming;
using Application.Dto.Streaming.Responses.SoundCloud;
using Application.Helpers.Contracts;
using Application.Strateges.Abstractions;
using Microsoft.Extensions.Options;

namespace Application.Strateges;

public class SoundCloudStrategy : IStreamingStrategy
{
    private const string BaseUrl = "https://api.soundcloud.com/";

    private readonly IHttpClientHelper _httpClientHelper;
    private readonly SoundCloudAuthOptions _options;

    public SoundCloudStrategy(IHttpClientHelper httpClientHelper, IOptions<SoundCloudAuthOptions> options)
    {
        _httpClientHelper = httpClientHelper;
        _options = options.Value;
    }

    public async Task AddTrackToPlaylistAsync(string playlistId, string trackId, string accessToken)
    {
        var endpoint = $"{BaseUrl}playlists/{playlistId}?oauth_token={accessToken}";
        var playlist = await _httpClientHelper.SendGetRequestAsync<SoundCloudPlaylistResponseDto>(endpoint, throwOnError: true);

        var updatedTrackIds = playlist.Tracks.Select(t => t.Id).Append(int.Parse(trackId)).ToList();
        var body = new { playlist = new { tracks = updatedTrackIds.Select(id => new { id }) } };

        await _httpClientHelper.SendPutJsonRequestAsync<object>(endpoint, body, throwOnError: true);
    }

    public async Task<string> CreatePlaylistAsync(string name, string accessToken, string? description = null, bool isPublic = false)
    {
        var endpoint = $"{BaseUrl}playlists?oauth_token={accessToken}";

        var body = new
        {
            playlist = new
            {
                title = name,
                description = description,
                sharing = isPublic ? "public" : "private"
            }
        };

        var response = await _httpClientHelper.SendPostJsonRequestAsync<SoundCloudPlaylistResponseDto>(endpoint, body, throwOnError: true);

        return response.Id.ToString();
    }

    public async Task<PageDto<SourceTrackDto>> GetLikedTracksAsync(LikedTracksPagedRequestDto dto, string accessToken)
    {
        var endpoint = $"{BaseUrl}me/favorites?limit={dto.PageSize}&offset={dto.Page * dto.PageSize}&oauth_token={accessToken}";
        var response = await _httpClientHelper.SendGetRequestAsync<SoundCloudLikedTracksResponseDto>(endpoint, throwOnError: true);

        var items = response.Collection.Select(x => new SourceTrackDto
        {
            SourceId = x.Id.ToString(),
            TrackName = x.Title,
            Artist = x.User?.Username,
            Album = x.PublisherMetadata?.AlbumTitle
        }).ToList();

        return new PageDto<SourceTrackDto>
        {
            Items = items,
            TotalCount = response.TotalCount
        };
    }

    public async Task<SourcePlaylistDto> GetPlaylistInfoAsync(string playlistId, string accessToken)
    {
        var endpoint = $"{BaseUrl}playlists/{playlistId}?oauth_token={accessToken}";

        var response = await _httpClientHelper
            .SendGetRequestAsync<SoundCloudPlaylistResponseDto>(endpoint, throwOnError: true);

        return new SourcePlaylistDto
        {
            Name = response.Title,
            Description = response.Description
        };
    }

    public async Task<IEnumerable<SourceTrackDto>> GetPlaylistTracksAsync(string playlistId, string accessToken)
    {
        var endpoint = $"{BaseUrl}playlists/{playlistId}?oauth_token={accessToken}";

        var response = await _httpClientHelper
            .SendGetRequestAsync<SoundCloudPlaylistResponseDto>(endpoint, throwOnError: true);

        return response.Tracks.Select(x => new SourceTrackDto
        {
            SourceId = x.Id.ToString(),
            TrackName = x.Title,
            Artist = x.User?.Username,
            Album = x.PublisherMetadata?.AlbumTitle
        });
    }

    public async Task<string?> SearchForTrackAsync(TrackSearchDto track, string accessToken)
    {
        var query = Uri.EscapeDataString($"{track.Artist} {track.Name}");
        var endpoint = $"{BaseUrl}tracks?client_id={_options.ClientId}&q={query}&limit=1";

        var response = await _httpClientHelper.SendGetRequestAsync<List<SoundCloudTrackDto>>(endpoint, throwOnError: true);
        return response.FirstOrDefault()?.Id.ToString();
    }
}
