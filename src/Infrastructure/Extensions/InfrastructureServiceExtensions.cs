using Domain.Entities;
using Infrastructure.Inspectors;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Identity;
using Infrastructure.Seeders;

namespace Infrastructure.Extensions;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddApplicationInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<SoftDeletableEntityInterceptor>();
        services.AddScoped<AuditableEntityInterceptor>();
        services.AddScoped<AuthorizationSeeder>();
        services.AddScoped<AdminUserSeeder>();

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            if (sp.GetRequiredService<IHostEnvironment>().IsDevelopment())
                options.EnableSensitiveDataLogging();

            options.UseNpgsql(configuration.GetConnectionString("WaveBridge"));
            options.AddInterceptors(
                sp.GetRequiredService<SoftDeletableEntityInterceptor>(),
                sp.GetRequiredService<AuditableEntityInterceptor>()
            );
            options.UseLazyLoadingProxies();
        });
        services.AddIdentityCore<User>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            })
            .AddRoles<Role>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped(typeof(IRepository<,>), typeof(ApplicationRepository<,>));

        return services;
    }
}
