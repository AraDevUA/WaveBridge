using Shared.Options;

namespace API.Extensions;

public static class CorsExtensions
{
    private const string RefreshedAccessTokenHeaderName = "X-Access-Token";

    public static IServiceCollection AddApiCors(this IServiceCollection services, IConfiguration configuration)
    {
        var corsOptions = configuration.GetSection(CorsOptions.SectionName).Get<CorsOptions>() ?? new CorsOptions();
        var allowedOrigins = corsOptions.AllowedOrigins
            .Where(origin => !string.IsNullOrWhiteSpace(origin))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        services.AddOptions<CorsOptions>()
            .Bind(configuration.GetSection(CorsOptions.SectionName));

        services.AddCors(options =>
        {
            options.AddPolicy(CorsOptions.PolicyName, policy =>
            {
                if (allowedOrigins.Length > 0)
                {
                    policy.WithOrigins(allowedOrigins);
                }

                if (corsOptions.AllowAnyHeader)
                {
                    policy.AllowAnyHeader();
                }

                policy.WithExposedHeaders(RefreshedAccessTokenHeaderName);

                if (corsOptions.AllowAnyMethod)
                {
                    policy.AllowAnyMethod();
                }

                if (corsOptions.AllowCredentials && allowedOrigins.Length > 0)
                {
                    policy.AllowCredentials();
                }
            });
        });

        return services;
    }
}
