using Application.Dto.Request.Transfers;
using Application.Dto.Streaming;
using Application.Results;
using Application.Results.Interfaces;
using Application.Services.Contracts;
using Application.Strateges.Abstractions;
using Domain.Entities;
using Infrastructure.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class TransferService : ITransferService
{
    private readonly IStreamingFacade _streamingFacade;
    private readonly IRepository<UserStreamingConnection, Guid> _userStreamingConnectionRepository;
    private readonly string _waveBridgeDescription = "This playlist was created by WaveBridge";
    public TransferService(IStreamingFacade streamingFacade, IRepository<UserStreamingConnection, Guid> userStreamingConnectionRepository)
    {
        _streamingFacade = streamingFacade;
        _userStreamingConnectionRepository = userStreamingConnectionRepository;
    }
    public async Task<IServiceResult> StartPlaylistTransferAsync(Guid userId, StartPlaylistTransferRequestDto dto)
    {
        var sourceConnection = await _userStreamingConnectionRepository.All
            .FirstOrDefaultAsync(x => x.UserId == userId && x.Service == dto.Source);
        var destinationConnection = await _userStreamingConnectionRepository.All
            .FirstOrDefaultAsync(x => x.UserId == userId && x.Service == dto.Destination);

        if (sourceConnection is null || destinationConnection is null)
            return ServiceResults.Failed("User is not connected to one of the services.");

        var sourceToken = await _streamingFacade.EnsureValidAccessTokenAsync(dto.Source, sourceConnection);
        var destinationToken = await _streamingFacade.EnsureValidAccessTokenAsync(dto.Destination, destinationConnection);

        var playlistInfo = await _streamingFacade.GetPlaylistInfoAsync(dto.Source, dto.SourcePlaylistId, sourceToken);
        var sourceTracks = await _streamingFacade.GetPlaylistTracksAsync(dto.Source, dto.SourcePlaylistId, sourceToken);

        if (!sourceTracks.Any())
            return ServiceResults.Failed("Source playlist is empty or not found.");

        var destinationPlaylistId = await _streamingFacade.CreatePlaylistAsync(dto.Destination, playlistInfo.Name, destinationToken, _waveBridgeDescription, dto.isPublic);

        foreach (var track in sourceTracks)
        {
            var trackId = await _streamingFacade.SearchForTrackAsync(dto.Destination, new TrackSearchDto
            {
                Name = track.TrackName,
                Artist = track.Artist,
                Album = track.Album
            }, destinationToken);

            if (trackId is null)
                continue;

            await _streamingFacade.AddTrackToPlaylistAsync(dto.Destination, destinationPlaylistId, trackId, destinationToken);
        }

        return ServiceResults.Ok(destinationPlaylistId);
    }
}
