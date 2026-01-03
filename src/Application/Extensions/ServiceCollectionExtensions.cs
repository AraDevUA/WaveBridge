using Application.Dto.Jwt;
using Application.Dto.Options;
using Application.Helpers;
using Application.Providers;
using Application.Providers.Contracts;
using Application.Services;
using Application.Services.Contracts;
using Infrastructure.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared.Options;

namespace Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;
        #region services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IGoogleAuthService, GoogleAuthService>();
        services.AddScoped<IJwtProvider, JwtProvider>();

        #endregion

        #region helpers
        services.AddHttpClient<HttpClientHelper>();
        #endregion
        services.AddOptions<AuthorizationOptions>()
            .Bind(configuration.GetSection("AuthorizationOptions"));
            
        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection("JwtOptions"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<GoogleAuthOptions>()
            .Bind(configuration.GetSection("OAuth:Google"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<EncryptionOptions>()
            .Bind(configuration.GetSection("Encryption"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddApplicationInfrastructure(configuration);
    }
}
