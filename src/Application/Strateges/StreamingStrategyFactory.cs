using Application.Strateges.Abstractions;
using Shared.Enums;

namespace Application.Strateges;

public class StreamingStrategyFactory : IStreamingStrategyFactory
{
    private readonly SpotifyStrategy _spotify;
    private readonly YouTubeMusicStrategy _youtubeMusic;
    private readonly SoundCloudStrategy _soundcloud;

    public StreamingStrategyFactory(SpotifyStrategy spotify, YouTubeMusicStrategy youtubeMusic, SoundCloudStrategy soundCloud)
    {
        _spotify = spotify;
        _youtubeMusic = youtubeMusic;
        _soundcloud = soundCloud;
    }

    public IStreamingStrategy GetStrategy(StreamingService platform) => platform switch
    {
        StreamingService.Spotify => _spotify,
        StreamingService.YouTubeMusic => _youtubeMusic,
        StreamingService.SoundCloud => _soundcloud,
        _ => throw new NotSupportedException($"Platform {platform} not supported")
    };
}
