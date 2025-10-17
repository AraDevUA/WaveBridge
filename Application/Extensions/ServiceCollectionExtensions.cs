using Application.Services;
using Application.Services.Contracts;
using Infrastructure.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        services.AddScoped<IUserService, UserService>();

        services.AddApplicationInfrastructure(builder.Configuration);
        return services;
    }
}
