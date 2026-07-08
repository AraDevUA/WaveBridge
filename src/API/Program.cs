using API.Extensions;
using API.Middlewares.Extensions;
using Application.Extensions;
using Infrastructure;
using Infrastructure.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Localization;
using Microsoft.OpenApi;
using Shared.Options;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

//var railwayPort = Environment.GetEnvironmentVariable("PORT");
//if (!string.IsNullOrWhiteSpace(railwayPort))
//{
//    builder.WebHost.UseUrls($"http://*:{railwayPort}");
//}

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(30);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });

builder.Services.AddHttpContextAccessor();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });

    c.AddSecurityRequirement(_ => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer", null),
            []
        }
    });
});

builder.Services.AddControllers();
builder.AddApplicationServices();

builder.Services.AddApiCors(builder.Configuration);
builder.Services.AddApiAuthentication(builder.Configuration);

builder.Services.AddOpenApi();

var app = builder.Build();

if (!app.Environment.IsEnvironment("Testing"))
{
    using var scope = app.Services.CreateScope();

    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();

    var seeder = scope.ServiceProvider.GetRequiredService<AuthorizationSeeder>();
    await seeder.SeedAsync();

    var adminUserSeeder = scope.ServiceProvider.GetRequiredService<AdminUserSeeder>();
    await adminUserSeeder.SeedAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();

}

app.UseExceptionHandlingMiddleware();

var supportedCultures = new[]
    {
        new CultureInfo("en-US"),
        new CultureInfo("ru-RU"),
        new CultureInfo("uk-UA"),
        new CultureInfo("de-DE")
    };

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en-US"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures,
    RequestCultureProviders = [new AcceptLanguageHeaderRequestCultureProvider()]
});

var enableHttpsRedirection = app.Configuration.GetValue("EnableHttpsRedirection", !app.Environment.IsDevelopment());

if (enableHttpsRedirection)
{
    app.UseHttpsRedirection();
}

app.UseCors(CorsOptions.PolicyName);
app.UseSession();

app.UseAuthentication();
app.UseRefreshTokenMiddleware();
app.UseAuthorization();

app.MapControllers();

app.Run();
