using Application.Dto.Requests.Transfers;
using Application.Dto.Responses.Transfers;
using Domain.Entities;

namespace Application.Dto.DtoExtensions;

public static class TransferDtoExtensions
{
    public static CreateTransferRequestDto ToCreateTransferRequestDto(this StartTransferRequestDto dto)
    {
        return new CreateTransferRequestDto
        {
            Source = dto.Source,
            IsPublic = dto.IsPublic,
            ToSinglePlaylist = dto.ToSinglePlaylist,
            Playlists = dto.Playlists
        };
    }

    public static ExecuteTransferRequestDto ToExecuteTransferRequestDto(this StartTransferRequestDto dto)
    {
        return new ExecuteTransferRequestDto
        {
            Destination = dto.Destination,
            IsPublic = dto.IsPublic,
            ToSinglePlaylist = dto.ToSinglePlaylist
        };
    }

    public static TransferDetailsResponseDto ToTransferDetailsResponseDto(this TransferOperation transferOperation, ICollection<TransferTrackResponseDto> failedTracks)
    {
        return new TransferDetailsResponseDto
        {
            Id = transferOperation.Id,
            Source = transferOperation.SourceService,
            Destination = transferOperation.TargetService,
            Status = transferOperation.Status,
            IsPublic = transferOperation.IsPublic,
            ToSinglePlaylist = transferOperation.ToSinglePlaylist,
            StartedUtc = transferOperation.StartedUtc,
            CompletedUtc = transferOperation.CompletedUtc,
            MergedTargetPlaylistId = transferOperation.MergedTargetPlaylistId,
            MergedTargetPlaylistUrl = transferOperation.MergedTargetPlaylistUrl,
            Playlists = transferOperation.TransferPlaylists.Select(x => x.ToTransferPlaylistResponseDto(transferOperation.ToSinglePlaylist)).ToList(),
            FailedTracks = failedTracks
        };
    }

    public static TransferPlaylistResponseDto ToTransferPlaylistResponseDto(this TransferPlaylist transferPlaylist, bool isMerged)
    {
        return new TransferPlaylistResponseDto
        {
            Id = transferPlaylist.Id,
            SourcePlaylistId = transferPlaylist.SourcePlaylistId,
            TargetPlaylistId = transferPlaylist.TargetPlaylistId,
            Name = transferPlaylist.Name,
            Description = transferPlaylist.Description,
            ArtworkUrl = transferPlaylist.ArtworkUrl,
            TargetPlaylistUrl = transferPlaylist.TargetPlaylistUrl,
            IsMerged = isMerged,
            Tracks = transferPlaylist.TransferTracks.Select(x => x.ToTransferTrackResponseDto()).ToList()
        };
    }

    public static TransferTrackResponseDto ToTransferTrackResponseDto(this TransferTrack transferTrack)
    {
        return new TransferTrackResponseDto
        {
            Id = transferTrack.Id,
            SourceId = transferTrack.SourceId,
            TargetId = transferTrack.TargetId,
            TrackName = transferTrack.TrackName,
            Artist = transferTrack.Artist,
            Album = transferTrack.Album,
            ArtworkUrl = transferTrack.ArtworkUrl,
            Status = transferTrack.Status,
            ErrorMessage = transferTrack.ErrorMessage
        };
    }
}
