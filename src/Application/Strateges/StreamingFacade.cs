using Application.Dto;
using Application.Dto.Requests.Transfers;
using Application.Dto.Streaming;
using Application.Strateges.Abstractions;
using Domain.Entities;
using Shared.Enums;
using Infrastructure.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Application.Strateges;

public class StreamingFacade : IStreamingFacade
{
    private readonly IStreamingStrategyFactory _factory;
    private readonly IRepository<UserStreamingConnection, Guid> _userStreamingConnectionRepository;

    public StreamingFacade(IStreamingStrategyFactory factory, IRepository<UserStreamingConnection, Guid> userStreamingConnectionRepository)
    {
        _factory = factory;
        _userStreamingConnectionRepository = userStreamingConnectionRepository;
    }
    public async Task<string> EnsureValidAccessTokenAsync(StreamingService platform, UserStreamingConnection connection)
    {
        if (connection.AccessTokenExpiresAtUtc > DateTimeOffset.UtcNow)
            return connection.AccessToken;

        if (_factory.GetStrategy(platform) is IStreamingAuthStrategy authStrategy && !string.IsNullOrEmpty(connection.RefreshToken))
        {
            var refreshedToken = await authStrategy.RefreshAsync(connection.RefreshToken);
            connection.AccessToken = refreshedToken.AccessToken;
            connection.RefreshToken = refreshedToken.RefreshToken ?? connection.RefreshToken;
            connection.AccessTokenExpiresAtUtc = DateTimeOffset.UtcNow.AddSeconds(refreshedToken.ExpiresIn);

            return connection.AccessToken;
        }

        return connection.AccessToken;
    }

    public async Task AddTrackToPlaylistAsync(StreamingService platform, string playlistId, string trackId, string accessToken)
    {
        var strategy = _factory.GetStrategy(platform);
        await strategy.AddTrackToPlaylistAsync(playlistId, trackId, accessToken);
    }
    public async Task<PageDto<SourceTrackDto>> GetLikedTracksAsync(StreamingService platform, LikedTracksPagedRequestDto pagedRequest, string accessToken)
    {
        var strategy = _factory.GetStrategy(platform);
        return await strategy.GetLikedTracksAsync(pagedRequest, accessToken);
    }

    public async Task<SourcePlaylistDto> GetPlaylistInfoAsync(StreamingService platform, string playlistId, string accessToken)
    {
        var strategy = _factory.GetStrategy(platform);
        return await strategy.GetPlaylistInfoAsync(playlistId, accessToken);
    }

    public async Task<IEnumerable<SourceTrackDto>> GetPlaylistTracksAsync(StreamingService platform, string playlistId, string accessToken)
    {
        var strategy = _factory.GetStrategy(platform);
        return await strategy.GetPlaylistTracksAsync(playlistId, accessToken);
    }

    public async Task<string?> SearchForTrackAsync(StreamingService platform, TrackSearchDto track, string accessToken)
    {
        var strategy = _factory.GetStrategy(platform);
        return await strategy.SearchForTrackAsync(track, accessToken);
    }

    public async Task<string> CreatePlaylistAsync(StreamingService platform, string name, string accessToken, string? description = null, bool isPublic = false)
    {
        var strategy = _factory.GetStrategy(platform);
        return await strategy.CreatePlaylistAsync(name, accessToken, description, isPublic);
    }
}
