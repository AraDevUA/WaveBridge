namespace Application.Helpers.Contracts;

public interface IHttpClientHelper
{
    Task<T> SendGetRequestAsync<T>(string endpoint, Dictionary<string, string>? queryParams = null, string? accessToken = null, bool throwOnError = false, CancellationToken cancellationToken = default);
    Task<T> SendPostFormRequestAsync<T>(string endpoint, Dictionary<string, string>? formParams = null, string? accessToken = null, bool throwOnError = false, CancellationToken cancellationToken = default);
    Task<T> SendPostJsonRequestAsync<T>(string endpoint, object body, string? accessToken = null, bool throwOnError = false, CancellationToken cancellationToken = default);
}