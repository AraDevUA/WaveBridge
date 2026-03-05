using Application.Strateges.Abstractions;
using Domain.Enums;

namespace Application.Strateges.Auth;

public class StreamingAuthFactory : IStreamingAuthFactory
{
    private readonly SpotifyAuthStrategy _spotify;
    private readonly YouTubeMusicAuthStrategy _youtubeMusic;
    private readonly SoundCloudAuthStrategy _soundcloud;

    public StreamingAuthFactory(
        SpotifyAuthStrategy spotify,
        YouTubeMusicAuthStrategy youtubeMusic,
        SoundCloudAuthStrategy soundCloud)
    {
        _spotify = spotify;
        _youtubeMusic = youtubeMusic;
        _soundcloud = soundCloud;
    }

    public IStreamingAuthStrategy GetStrategy(StreamingService service) => service switch
    {
        StreamingService.Spotify => _spotify,
        StreamingService.YouTubeMusic => _youtubeMusic,
        StreamingService.SoundCloud => _soundcloud,
        _ => throw new NotSupportedException($"Platform {service} not supported")
    };
}
