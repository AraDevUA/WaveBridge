using Application.Dto;
using Application.Dto.DtoExtensions;
using Application.Dto.Requests.Transfers;
using Application.Dto.Responses.Transfers;
using Application.Dto.Streaming;
using Application.Extensions;
using Application.Localization;
using Application.Results;
using Application.Results.Interfaces;
using Application.Services.Contracts;
using Application.Strateges.Abstractions;
using Domain.Entities;
using Infrastructure.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;
using Shared.Enums;

namespace Application.Services;

public class TransferService : ITransferService
{
    private const string WaveBridgeDescription = "This playlist was created by WaveBridge";
    private const string DefaultPlaylistName = "WaveBridge Playlist";

    private readonly IStreamingFacade _streamingFacade;
    private readonly IRepository<UserStreamingConnection, Guid> _userStreamingConnectionRepository;
    private readonly IRepository<TransferOperation, Guid> _transferOperationRepository;

    public TransferService(
        IStreamingFacade streamingFacade,
        IRepository<UserStreamingConnection, Guid> userStreamingConnectionRepository,
        IRepository<TransferOperation, Guid> transferOperationRepository)
    {
        _streamingFacade = streamingFacade;
        _userStreamingConnectionRepository = userStreamingConnectionRepository;
        _transferOperationRepository = transferOperationRepository;
    }

    public async Task<IServiceResult> StartTransferAsync(Guid userId, StartTransferRequestDto dto, CancellationToken cancellationToken = default)
    {
        var createResult = await CreateTransferOperationAsync(userId, dto.ToCreateTransferRequestDto(), cancellationToken);
        if (!createResult.IsSuccess)
            return ServiceResults.Failed(createResult.ErrorMessage ?? SystemMessages.TransferCreationFailed);

        var transferOperation = createResult.TransferOperation!;

        var executeResult = await ExecuteTransferAsync(userId, transferOperation, dto.ToExecuteTransferRequestDto(), cancellationToken);
        if (!executeResult.IsSuccess)
            return ServiceResults.Failed(executeResult.ErrorMessage ?? SystemMessages.TransferExecutionFailed);

        var result = transferOperation;
        return ServiceResults.Ok(result.ToTransferDetailsResponseDto(GetFailedTracks(result)));
    }

    public async Task<IServiceResult> GetPlaylistsAsync(Guid userId, StreamingService source, PagedRequest pagedRequest, CancellationToken cancellationToken = default)
    {
        var connection = await GetUserConnectionOrNullAsync(userId, source, cancellationToken);
        if (connection is null)
            return ServiceResults.Failed(string.Format(SystemMessages.UserNotConnected, source));

        var accessToken = await _streamingFacade.EnsureValidAccessTokenAsync(source, connection);
        var playlists = await _streamingFacade.GetPlaylistsAsync(source, pagedRequest, accessToken);
        return ServiceResults.Ok(playlists);
    }

    public async Task<IServiceResult> GetPlaylistTracksAsync(Guid userId, StreamingService source, string playlistId, CancellationToken cancellationToken = default)
    {
        var connection = await GetUserConnectionOrNullAsync(userId, source, cancellationToken);
        if (connection is null)
            return ServiceResults.Failed(string.Format(SystemMessages.UserNotConnected, source));

        var accessToken = await _streamingFacade.EnsureValidAccessTokenAsync(source, connection);
        var tracks = await _streamingFacade.GetPlaylistTracksAsync(source, playlistId, accessToken);
        return ServiceResults.Ok(tracks);
    }

    public async Task<IServiceResult> GetLikedTracksAsync(Guid userId, StreamingService source, LikedTracksPagedRequestDto dto, CancellationToken cancellationToken = default)
    {
        var connection = await GetUserConnectionOrNullAsync(userId, source, cancellationToken);
        if (connection is null)
            return ServiceResults.Failed(string.Format(SystemMessages.UserNotConnected, source));

        var accessToken = await _streamingFacade.EnsureValidAccessTokenAsync(source, connection);
        var tracks = await _streamingFacade.GetLikedTracksAsync(source, dto, accessToken);
        return ServiceResults.Ok(tracks);
    }
    public async Task<IServiceResult> GetTransferHistoryAsync(Guid userId, TransferHistoryRequestDto dto, CancellationToken cancellationToken = default)
    {
        var query = _transferOperationRepository.All
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedUtc);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Paginate(dto.Page, dto.PageSize)
            .Select(x => new TransferHistoryItemResponseDto
            {
                Id = x.Id,
                SourceService = x.SourceService,
                TargetService = x.TargetService,
                Status = x.Status,
                IsPublic = x.IsPublic,
                ToSinglePlaylist = x.ToSinglePlaylist,
                MergedTargetPlaylistUrl = x.MergedTargetPlaylistUrl,
                StartedUtc = x.StartedUtc,
                CompletedUtc = x.CompletedUtc,
                PlaylistCount = x.TransferPlaylists.Count
            })
            .ToListAsync(cancellationToken);

        var result = new PageDto<TransferHistoryItemResponseDto>()
        {
            Items = items,
            TotalCount = totalCount
        };

        return ServiceResults.Ok(result);
    }
    private async Task<TransferCreationResult> CreateTransferOperationAsync(Guid userId, CreateTransferRequestDto dto, CancellationToken cancellationToken = default)
    {
        if (!dto.Playlists.Any())
            return TransferCreationResult.Failure(SystemMessages.AtLeastOnePlaylistRequired);

        var sourceConnection = await GetUserConnectionOrNullAsync(userId, dto.Source, cancellationToken);
        if (sourceConnection is null)
            return TransferCreationResult.Failure(string.Format(SystemMessages.UserNotConnected, dto.Source));

        var transferOperation = new TransferOperation
        {
            UserId = userId,
            SourceService = dto.Source,
            Status = TransferStatus.Queued,
            IsPublic = dto.IsPublic,
            ToSinglePlaylist = dto.ToSinglePlaylist
        };

        foreach (var playlist in dto.Playlists)
        {
            if (playlist.Tracks.Count == 0)
                continue;

            var transferPlaylist = new TransferPlaylist
            {
                SourcePlaylistId = playlist.SourcePlaylistId,
                Name = playlist.Name,
                Description = playlist.Description,
                ArtworkUrl = playlist.ArtworkUrl
            };

            foreach (var track in playlist.Tracks)
            {
                transferPlaylist.TransferTracks.Add(new TransferTrack
                {
                    SourceId = track.SourceId,
                    TrackName = track.TrackName,
                    Artist = track.Artist,
                    Album = track.Album,
                    ArtworkUrl = track.ArtworkUrl,
                    Status = TransferTrackStatus.Pending
                });
            }

            transferOperation.TransferPlaylists.Add(transferPlaylist);
        }

        if (!transferOperation.TransferPlaylists.Any())
            return TransferCreationResult.Failure(SystemMessages.SelectedPlaylistsContainNoTracks);

        await _transferOperationRepository.CreateAsync(transferOperation, cancellationToken);
        return TransferCreationResult.Success(transferOperation);
    }

    private async Task<TransferExecutionResult> ExecuteTransferAsync(Guid userId, TransferOperation transferOperation, ExecuteTransferRequestDto dto, CancellationToken cancellationToken = default)
    {
        var sourceConnection = await GetUserConnectionOrNullAsync(userId, transferOperation.SourceService, cancellationToken);
        if (sourceConnection is null)
            return TransferExecutionResult.Failure(string.Format(SystemMessages.UserNotConnected, transferOperation.SourceService));

        var destinationConnection = await GetUserConnectionOrNullAsync(userId, dto.Destination, cancellationToken);
        if (destinationConnection is null)
            return TransferExecutionResult.Failure(string.Format(SystemMessages.UserNotConnected, dto.Destination));

        try
        {
            var destinationToken = await _streamingFacade.EnsureValidAccessTokenAsync(dto.Destination, destinationConnection);

            transferOperation.TargetService = dto.Destination;
            transferOperation.IsPublic = dto.IsPublic ?? transferOperation.IsPublic;
            transferOperation.ToSinglePlaylist = dto.ToSinglePlaylist ?? transferOperation.ToSinglePlaylist;
            transferOperation.Status = TransferStatus.InProgress;
            transferOperation.StartedUtc = DateTimeOffset.UtcNow;
            transferOperation.CompletedUtc = null;

            await _transferOperationRepository.SaveChangesAsync();

            if (transferOperation.ToSinglePlaylist)
            {
                await ExecuteMergedTransferAsync(transferOperation, dto.Destination, destinationToken);
            }
            else
            {
                foreach (var playlist in transferOperation.TransferPlaylists)
                {
                    await ExecutePlaylistTransferAsync(dto.Destination, destinationToken, playlist, transferOperation.IsPublic);
                }
            }

            transferOperation.Status = transferOperation.TransferPlaylists
                .SelectMany(x => x.TransferTracks)
                .Any(x => x.Status is TransferTrackStatus.Failed or TransferTrackStatus.NotFound)
                ? TransferStatus.Failed
                : TransferStatus.Completed;

            transferOperation.CompletedUtc = DateTimeOffset.UtcNow;
            await _transferOperationRepository.SaveChangesAsync();

            return TransferExecutionResult.Success();
        }
        catch (Exception ex)
        {
            transferOperation.Status = TransferStatus.Failed;
            transferOperation.CompletedUtc = DateTimeOffset.UtcNow;

            foreach (var pendingTrack in transferOperation.TransferPlaylists
                         .SelectMany(x => x.TransferTracks)
                         .Where(x => x.Status == TransferTrackStatus.Pending))
            {
                pendingTrack.Status = TransferTrackStatus.Failed;
                pendingTrack.ErrorMessage = ex.Message;
            }

            await _transferOperationRepository.SaveChangesAsync();
            return TransferExecutionResult.Failure(ex.Message);
        }
    }

    public async Task<IServiceResult> GetTransferAsync(Guid userId, Guid transferId, CancellationToken cancellationToken = default)
    {
        var transferOperation = await GetTransferOperationAsync(userId, transferId, cancellationToken);
        if (transferOperation is null)
            return ServiceResults.NotFound();

        var result = transferOperation;
        return ServiceResults.Ok(result.ToTransferDetailsResponseDto(GetFailedTracks(result)));
    }

    private async Task ExecutePlaylistTransferAsync(StreamingService destination, string destinationToken, TransferPlaylist playlist, bool isPublic)
    {
        var destinationPlaylist = await _streamingFacade.CreatePlaylistAsync(
            destination,
            playlist.Name,
            destinationToken,
            NormalizePlaylistDescription(playlist.Description),
            isPublic);

        playlist.TargetPlaylistId = destinationPlaylist.SourceId;
        playlist.TargetPlaylistUrl = destinationPlaylist.Url;

        foreach (var track in playlist.TransferTracks)
        {
            await TransferTrackAsync(track, destination, destinationToken, playlist.TargetPlaylistId);
        }
    }

    private async Task ExecuteMergedTransferAsync(TransferOperation transferOperation, StreamingService destination, string destinationToken)
    {
        var mergedPlaylist = await EnsureMergedPlaylistAsync(transferOperation, destination, destinationToken);

        foreach (var track in transferOperation.TransferPlaylists.SelectMany(x => x.TransferTracks))
        {
            await TransferTrackAsync(track, destination, destinationToken, mergedPlaylist.SourceId);
        }

        foreach (var playlist in transferOperation.TransferPlaylists)
        {
            playlist.TargetPlaylistId = mergedPlaylist.SourceId;
            playlist.TargetPlaylistUrl = mergedPlaylist.Url;
        }
    }

    private async Task<SourcePlaylistDto> EnsureMergedPlaylistAsync(TransferOperation transferOperation, StreamingService destination, string destinationToken)
    {
        if (!string.IsNullOrWhiteSpace(transferOperation.MergedTargetPlaylistId))
        {
            return new SourcePlaylistDto
            {
                SourceId = transferOperation.MergedTargetPlaylistId,
                Url = transferOperation.MergedTargetPlaylistUrl,
                Name = DefaultPlaylistName
            };
        }

        var playlist = await _streamingFacade.CreatePlaylistAsync(
            destination,
            DefaultPlaylistName,
            destinationToken,
            WaveBridgeDescription,
            transferOperation.IsPublic);

        transferOperation.MergedTargetPlaylistId = playlist.SourceId;
        transferOperation.MergedTargetPlaylistUrl = playlist.Url;
        await _transferOperationRepository.SaveChangesAsync();

        return playlist;
    }

    private async Task TransferTrackAsync(TransferTrack track, StreamingService destination, string destinationToken, string? destinationPlaylistId)
    {
        if (string.IsNullOrWhiteSpace(destinationPlaylistId))
        {
            track.Status = TransferTrackStatus.Failed;
            track.ErrorMessage = SystemMessages.DestinationPlaylistWasNotCreated;
            return;
        }

        var trackId = await FindTrackIdWithFallbackAsync(track, destination, destinationToken);
        if (trackId is null)
        {
            track.Status = TransferTrackStatus.NotFound;
            track.ErrorMessage = SystemMessages.TrackWasNotFoundOnDestinationPlatform;
            return;
        }

        await _streamingFacade.AddTrackToPlaylistAsync(destination, destinationPlaylistId, trackId, destinationToken);
        track.TargetId = trackId;
        track.Status = TransferTrackStatus.Transferred;
        track.ErrorMessage = null;
    }

    private async Task<string?> FindTrackIdWithFallbackAsync(TransferTrack track, StreamingService service, string accessToken)
    {
        var trackId = await _streamingFacade.SearchForTrackAsync(service, new TrackSearchDto
        {
            Name = track.TrackName,
            Artist = track.Artist,
            Album = track.Album
        }, accessToken);

        if (trackId is not null)
            return trackId;

        return await _streamingFacade.SearchForTrackAsync(service, new TrackSearchDto
        {
            Name = track.TrackName,
            Album = track.Album,
            Artist = string.Empty
        }, accessToken);
    }

    private async Task<UserStreamingConnection?> GetUserConnectionOrNullAsync(Guid userId, StreamingService service, CancellationToken cancellationToken = default)
    {
        return await _userStreamingConnectionRepository.All
            .FirstOrDefaultAsync(x => x.UserId == userId && x.Service == service, cancellationToken);
    }

    private async Task<TransferOperation?> GetTransferOperationAsync(Guid userId, Guid transferId, CancellationToken cancellationToken = default)
    {
        return await _transferOperationRepository.All
            .AsNoTracking()
            .Include(x => x.TransferPlaylists)
                .ThenInclude(x => x.TransferTracks)
            .FirstOrDefaultAsync(x => x.Id == transferId && x.UserId == userId, cancellationToken);
    }

    private static string NormalizePlaylistDescription(string? description)
    {
        return string.IsNullOrWhiteSpace(description)
            ? WaveBridgeDescription
            : $"{description}\n\n{WaveBridgeDescription}";
    }

    private static ICollection<TransferTrackResponseDto> GetFailedTracks(TransferOperation transferOperation)
    {
        var allTracks = transferOperation.TransferPlaylists
            .SelectMany(x => x.TransferTracks)
            .Select(x => x.ToTransferTrackResponseDto())
            .ToList();

        return allTracks
            .Where(x => x.Status is TransferTrackStatus.NotFound or TransferTrackStatus.Failed)
            .ToList();
    }
}
