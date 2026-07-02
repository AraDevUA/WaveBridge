using Application.Dto;
using Application.Dto.Requests.Transfers;
using Application.Dto.Streaming;
using Application.Strateges.Abstractions;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

namespace Application.Strateges;

public class YouTubeMusicStrategy : IStreamingStrategy
{
    public async Task AddTrackToPlaylistAsync(string playlistId, string trackId, string accessToken)
    {
        var service = CreateYouTubeService(accessToken);
        var item = new PlaylistItem
        {
            Snippet = new PlaylistItemSnippet
            {
                PlaylistId = playlistId,
                ResourceId = new ResourceId
                {
                    Kind = "youtube#video",
                    VideoId = trackId
                }
            }
        };

        await service.PlaylistItems.Insert(item, "snippet").ExecuteAsync();
    }

    public async Task<PageDto<SourceTrackDto>> GetLikedTracksAsync(LikedTracksPagedRequestDto dto, string accessToken)
    {
        var service = CreateYouTubeService(accessToken);
        var request = service.Videos.List("snippet,contentDetails");
        request.MyRating = VideosResource.ListRequest.MyRatingEnum.Like;
        request.MaxResults = dto.PageSize;
        request.PageToken = dto.PageToken;

        var response = await request.ExecuteAsync();

        return new PageDto<SourceTrackDto>
        {
            Items = response.Items.Select(video => new SourceTrackDto
            {
                SourceId = video.Id,
                TrackName = video.Snippet.Title,
                Artist = video.Snippet.ChannelTitle,
                Album = string.Empty,
                ArtworkUrl = video.Snippet.Thumbnails?.Medium?.Url ?? video.Snippet.Thumbnails?.Default__?.Url
            }).ToList(),
            TotalCount = (int)response.PageInfo.TotalResults
        };
    }

    public async Task<PageDto<SourcePlaylistDto>> GetPlaylistsAsync(PagedRequest dto, string accessToken)
    {
        var service = CreateYouTubeService(accessToken);
        var request = service.Playlists.List("snippet,contentDetails");
        request.Mine = true;
        request.MaxResults = dto.PageSize;

        var response = await request.ExecuteAsync();

        return new PageDto<SourcePlaylistDto>
        {
            Items = response.Items.Select(playlist => new SourcePlaylistDto
            {
                SourceId = playlist.Id,
                Name = playlist.Snippet.Title,
                Description = playlist.Snippet.Description,
                ArtworkUrl = playlist.Snippet.Thumbnails?.Medium?.Url ?? playlist.Snippet.Thumbnails?.Default__?.Url,
                Url = $"https://music.youtube.com/playlist?list={playlist.Id}",
                TrackCount = (int?)playlist.ContentDetails?.ItemCount
            }).ToList(),
            TotalCount = (int)response.PageInfo.TotalResults
        };
    }

    public async Task<IEnumerable<SourceTrackDto>> GetPlaylistTracksAsync(string playlistId, string accessToken)
    {
        var service = CreateYouTubeService(accessToken);
        var request = service.PlaylistItems.List("snippet,contentDetails");
        request.PlaylistId = playlistId;

        var response = await request.ExecuteAsync();

        return response.Items.Select(x => new SourceTrackDto
        {
            SourceId = x.ContentDetails.VideoId,
            TrackName = x.Snippet.Title,
            Artist = x.Snippet.VideoOwnerChannelTitle ?? x.Snippet.ChannelTitle,
            Album = string.Empty,
            ArtworkUrl = x.Snippet.Thumbnails?.Medium?.Url ?? x.Snippet.Thumbnails?.Default__?.Url
        }).ToList();
    }

    public async Task<string?> SearchForTrackAsync(TrackSearchDto track, string accessToken)
    {
        try
        {
            var service = CreateYouTubeService(accessToken);
            var request = service.Search.List("snippet");
            request.Q = $"{track.Name} {track.Artist}";
            if (!string.IsNullOrEmpty(track.Album))
                request.Q += $" {track.Album}";
            request.Type = "video";
            request.MaxResults = 1;

            var response = await request.ExecuteAsync();
            return response.Items.FirstOrDefault()?.Id.VideoId;
        }
        catch (Google.GoogleApiException ex) when (ex.HttpStatusCode is System.Net.HttpStatusCode.Forbidden)
        {
            throw new InvalidOperationException("YouTube API quota exceeded.", ex);
        }
    }

    public async Task<SourcePlaylistDto> CreatePlaylistAsync(string name, string accessToken, string? description = null, bool isPublic = false)
    {
        var service = CreateYouTubeService(accessToken);
        var playlist = new Playlist
        {
            Snippet = new PlaylistSnippet
            {
                Title = name,
                Description = description ?? string.Empty
            },
            Status = new PlaylistStatus
            {
                PrivacyStatus = isPublic ? "public" : "private"
            }
        };

        var response = await service.Playlists.Insert(playlist, "snippet,status").ExecuteAsync();

        return new SourcePlaylistDto
        {
            SourceId = response.Id,
            Name = response.Snippet.Title,
            Description = response.Snippet.Description,
            ArtworkUrl = response.Snippet.Thumbnails?.Medium?.Url ?? response.Snippet.Thumbnails?.Default__?.Url,
            Url = $"https://music.youtube.com/playlist?list={response.Id}"
        };
    }

    public async Task<SourcePlaylistDto> GetPlaylistInfoAsync(string playlistId, string accessToken)
    {
        var service = CreateYouTubeService(accessToken);
        var request = service.Playlists.List("snippet,contentDetails");
        request.Id = playlistId;

        var response = await request.ExecuteAsync();
        var playlist = response.Items.First();

        return new SourcePlaylistDto
        {
            SourceId = playlist.Id,
            Name = playlist.Snippet.Title,
            Description = playlist.Snippet.Description,
            ArtworkUrl = playlist.Snippet.Thumbnails?.Medium?.Url ?? playlist.Snippet.Thumbnails?.Default__?.Url,
            Url = $"https://music.youtube.com/playlist?list={playlist.Id}",
            TrackCount = (int?)playlist.ContentDetails?.ItemCount
        };
    }

    private static YouTubeService CreateYouTubeService(string accessToken)
    {
        return new YouTubeService(new BaseClientService.Initializer
        {
            HttpClientInitializer = GoogleCredential.FromAccessToken(accessToken),
            ApplicationName = "WaveBridge"
        });
    }
}
