using Application.Dto.Jwt;
using Application.Dto.Options;
using Application.Dto.Options.Auth.Google;
using Application.Dto.Options.Auth.Spotify;
using Application.Helpers;
using Application.Helpers.Contracts;
using Application.Providers;
using Application.Providers.Contracts;
using Application.Services;
using Application.Services.Contracts;
using Application.Strateges;
using Application.Strateges.Abstractions;
using Application.Strateges.Auth;
using Infrastructure.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared.Options;
using FluentValidation;
using Application.Dto.Options.Auth.SoundCloud;

namespace Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;

        services.AddValidatorsFromAssembly(typeof(ServiceCollectionExtensions).Assembly);

        #region services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IGoogleAuthService, GoogleAuthService>();
        services.AddScoped<IJwtProvider, JwtProvider>();
        services.AddScoped<ITransferService, TransferService>();
        services.AddScoped<IStreamingAuthService, StreamingAuthService>();
        #endregion
        #region strateges
        services.AddScoped<YouTubeMusicStrategy>();
        services.AddScoped<SpotifyStrategy>();
        services.AddScoped<SpotifyAuthStrategy>();
        services.AddScoped<YouTubeMusicAuthStrategy>();
        services.AddScoped<SoundCloudAuthStrategy>();
        services.AddScoped<SoundCloudStrategy>();

        #endregion
        #region helpers
        services.AddHttpClient<IHttpClientHelper, HttpClientHelper>();
        #endregion
        services.AddScoped<IStreamingFacade, StreamingFacade>();
        services.AddScoped<IStreamingStrategyFactory, StreamingStrategyFactory>();
        services.AddScoped<IStreamingAuthFacade, StreamingAuthFacade>();
        services.AddScoped<IStreamingAuthFactory, StreamingAuthFactory>();
        services.AddScoped<IOAuthStateProvider, OAuthStateProvider>();


        services.AddOptions<AuthorizationOptions>()
            .Bind(configuration.GetSection("AuthorizationOptions"));

        services.AddOptions<AdminUserOptions>()
            .Bind(configuration.GetSection("AdminUser"));

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

        services.AddOptions<FrontendOptions>()
            .Bind(configuration.GetSection(FrontendOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<SpotifyAuthOptions>()
            .Bind(configuration.GetSection("OAuth:Spotify"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<SoundCloudAuthOptions>()
            .Bind(configuration.GetSection("OAuth:SoundCloud"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddApplicationInfrastructure(configuration);
    }
}
