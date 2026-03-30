using Shared.Enums;

namespace Application.Strateges.Abstractions;

public interface IStreamingAuthFactory
{
    IStreamingAuthStrategy GetStrategy(StreamingService service);
}