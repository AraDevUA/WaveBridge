using Shared.Enums;

namespace Application.Strateges.Abstractions;

public interface IStreamingStrategyFactory
{
    IStreamingStrategy GetStrategy(StreamingService platform);
}
