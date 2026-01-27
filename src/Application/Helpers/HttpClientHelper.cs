using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Application.Helpers;

public class HttpClientHelper
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<HttpClientHelper> _logger;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public HttpClientHelper(HttpClient httpClient, ILogger<HttpClientHelper> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<T> SendGetRequestAsync<T>(string endpoint, Dictionary<string, string>? queryParams = null, string? accessToken = null, bool throwOnError = false, CancellationToken cancellationToken = default)
    {
        return await SendHttpRequestAsync<T>(HttpMethod.Get, endpoint, accessToken, queryParams, null, throwOnError, cancellationToken);
    }

    public async Task<T> SendPostFormRequestAsync<T>(string endpoint, Dictionary<string, string>? formParams = null, string? accessToken = null, bool throwOnError = false, CancellationToken cancellationToken = default)
    {
        var httpContent = formParams != null ? new FormUrlEncodedContent(formParams) : null;

        return await SendHttpRequestAsync<T>(HttpMethod.Post, endpoint, accessToken, null, httpContent, throwOnError, cancellationToken);
    }

    public async Task<T> SendPostJsonRequestAsync<T>(string endpoint, object body, string? accessToken = null, bool throwOnError = false, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(body, _jsonOptions);
        var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

        return await SendHttpRequestAsync<T>(HttpMethod.Post, endpoint, accessToken, null, httpContent, throwOnError, cancellationToken);
    }

    private async Task<T> SendHttpRequestAsync<T>(
        HttpMethod httpMethod,
        string endpoint,
        string? accessToken = null,
        Dictionary<string, string>? queryParams = null,
        HttpContent? httpContent = null,
        bool throwOnError = false,
        CancellationToken cancellationToken = default)
    {
        var url = queryParams != null
            ? QueryHelpers.AddQueryString(endpoint, queryParams)
            : endpoint;

        using var request = new HttpRequestMessage(httpMethod, url);

        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        if (!string.IsNullOrWhiteSpace(accessToken))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        if (httpContent != null)
            request.Content = httpContent;

        using var response = await _httpClient.SendAsync(request, cancellationToken);

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("HTTP request failed. StatusCode: {StatusCode}, URL: {Url}, Response: {Response}", response.StatusCode, url, responseContent);

            if (throwOnError)
                throw new HttpRequestException($"Request to {url} failed with status {(int)response.StatusCode}: {responseContent}");
        }

        if (string.IsNullOrWhiteSpace(responseContent))
            return default!;

        return JsonSerializer.Deserialize<T>(responseContent, _jsonOptions)!;
    }

}
