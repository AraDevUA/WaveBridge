using Application.Strateges.Abstractions;
using Domain.Enums;

namespace Application.Strateges.Auth;

public class StreamingAuthFactory : IStreamingAuthFactory
{
    private readonly SpotifyAuthStrategy _spotify;
    private readonly YouTubeMusicAuthStrategy _youtubeMusic;

    public StreamingAuthFactory(
        SpotifyAuthStrategy spotify,
        YouTubeMusicAuthStrategy youtubeMusic)
    {
        _spotify = spotify;
        _youtubeMusic = youtubeMusic;
    }

    public IStreamingAuthStrategy GetStrategy(StreamingService service) => service switch
    {
        StreamingService.Spotify => _spotify,
        StreamingService.YouTubeMusic => _youtubeMusic,
        _ => throw new NotSupportedException($"Platform {service} not supported")
    };
}
