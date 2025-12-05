using Application.Dto.Options;
using Application.Helpers;
using Application.Providers;
using Application.Providers.Contracts;
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
        var configuration = builder.Configuration;
        #region services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IJwtProvider, JwtProvider>();

        #endregion

        #region helpers
        services.AddHttpClient<HttpClientHelper>();
        #endregion

        services.Configure<GoogleAuthOptions>(configuration.GetSection("Authentication:Google"));
        services.Configure<EncryptionOptions>(configuration.GetSection("Encryption"));

        services.AddApplicationInfrastructure(configuration);
        return services;
    }
}
