namespace API.Middlewares.Extensions;

public static class RefreshTokenMiddlewareExtensions
{
    public static IApplicationBuilder UseRefreshTokenMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RefreshTokenMiddleware>();
    }
}
