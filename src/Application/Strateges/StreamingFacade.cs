using Application.Dto;
using Application.Dto.Options;
using Application.Dto.Requests.Transfers;
using Application.Dto.Streaming;
using Application.Helpers;
using Application.Strateges.Abstractions;
using Domain.Entities;
using Shared.Enums;
using Infrastructure.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace Application.Strateges;

public class StreamingFacade : IStreamingFacade
{
    private readonly IStreamingStrategyFactory _factory;
    private readonly IRepository<UserStreamingConnection, Guid> _userStreamingConnectionRepository;
    private readonly EncryptionOptions _encryptionOptions;

    public StreamingFacade(
        IStreamingStrategyFactory factory,
        IRepository<UserStreamingConnection, Guid> userStreamingConnectionRepository,
        IOptions<EncryptionOptions> encryptionOptions)
    {
        _factory = factory;
        _userStreamingConnectionRepository = userStreamingConnectionRepository;
        _encryptionOptions = encryptionOptions.Value;
    }
    public async Task<string> EnsureValidAccessTokenAsync(StreamingService platform, UserStreamingConnection connection)
    {
        if (connection.AccessTokenExpiresAtUtc > DateTimeOffset.UtcNow)
            return connection.AccessToken;

        if (_factory.GetStrategy(platform) is IStreamingAuthStrategy authStrategy && !string.IsNullOrEmpty(connection.RefreshToken))
        {
            var decryptedRefreshToken = DecryptRefreshToken(connection.RefreshToken);
            var refreshedToken = await authStrategy.RefreshAsync(decryptedRefreshToken);
            connection.AccessToken = refreshedToken.AccessToken;
            connection.RefreshToken = EncryptRefreshToken(refreshedToken.RefreshToken ?? decryptedRefreshToken);
            connection.AccessTokenExpiresAtUtc = DateTimeOffset.UtcNow.AddSeconds(refreshedToken.ExpiresIn);
            await _userStreamingConnectionRepository.SaveChangesAsync();

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

    public async Task<PageDto<SourcePlaylistDto>> GetPlaylistsAsync(StreamingService platform, PagedRequest pagedRequest, string accessToken)
    {
        var strategy = _factory.GetStrategy(platform);
        return await strategy.GetPlaylistsAsync(pagedRequest, accessToken);
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

    public async Task<SourcePlaylistDto> CreatePlaylistAsync(StreamingService platform, string name, string accessToken, string? description = null, bool isPublic = false)
    {
        var strategy = _factory.GetStrategy(platform);
        return await strategy.CreatePlaylistAsync(name, accessToken, description, isPublic);
    }

    private string EncryptRefreshToken(string refreshToken)
    {
        return AesGcmHelper.Encrypt(refreshToken, _encryptionOptions.KeyBase64);
    }

    private string DecryptRefreshToken(string refreshToken)
    {
        try
        {
            return AesGcmHelper.Decrypt(refreshToken, _encryptionOptions.KeyBase64);
        }
        catch (FormatException)
        {
            return refreshToken;
        }
        catch (ArgumentException)
        {
            return refreshToken;
        }
        catch (CryptographicException)
        {
            return refreshToken;
        }
    }
}
