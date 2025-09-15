using Infrastructure.Inspectors;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.EnableSensitiveDataLogging();
            options.UseNpgsql(configuration.GetConnectionString("WaveBridge"));
            options.AddInterceptors(
                new SoftDeletableEntityInterceptor(),
                new AuditableEntityInterceptor()
            );
            options.UseLazyLoadingProxies();
        });

        services.AddRepositories();

        return services;
    }
    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<,>), typeof(ApplicationRepository<,>));
    }
}
