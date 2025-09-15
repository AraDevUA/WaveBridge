using Infrastructure.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Application.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        services.AddApplicationInfrastructure(builder.Configuration);
        return services;
    }
}
