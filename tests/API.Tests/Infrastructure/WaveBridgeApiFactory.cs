using Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace API.Tests.Infrastructure;

public class WaveBridgeApiFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"WaveBridge.Api.Tests.{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["EnableHttpsRedirection"] = "false",
                ["JwtOptions:Key"] = "test-secret-key-with-at-least-32-bytes",
                ["JwtOptions:Issuer"] = "WaveBridge.Tests",
                ["JwtOptions:Audience"] = "WaveBridge.Tests",
                ["JwtOptions:ExpiresHours"] = "1",
                ["JwtOptions:RefreshTokenLifetimeDays"] = "7"
            });
        });

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<ApplicationDbContext>>();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase(_databaseName));
        });
    }

    public async Task ResetDatabaseAsync(CancellationToken cancellationToken = default)
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await dbContext.Database.EnsureDeletedAsync(cancellationToken);
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);
    }
}
