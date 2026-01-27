using Application.Strateges.Abstractions;
using Domain.Enums;

namespace Application.Strateges;

public class StreamingStrategyFactory : IStreamingStrategyFactory
{
    private readonly SpotifyStrategy _spotify;
    private readonly YouTubeMusicStrategy _youtubeMusic;

    public StreamingStrategyFactory(SpotifyStrategy spotify, YouTubeMusicStrategy youtubeMusic)
    {
        _spotify = spotify;
        _youtubeMusic = youtubeMusic;
    }

    public IStreamingStrategy GetStrategy(StreamingService platform) => platform switch
    {
        StreamingService.Spotify => _spotify,
        StreamingService.YouTubeMusic => _youtubeMusic,
        _ => throw new NotSupportedException($"Platform {platform} not supported")
    };
}
