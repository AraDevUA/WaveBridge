using Application.Providers.Contracts;
using Application.Strateges.Abstractions;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Application.Strateges.Auth;

public class StreamingAuthFacade : IStreamingAuthFacade
{
    private readonly IStreamingAuthFactory _factory;
    private readonly IRepository<UserStreamingConnection, Guid> _userStreamingConnectionRepository;
    private readonly IOAuthStateProvider _stateProvider;
    public StreamingAuthFacade(IStreamingAuthFactory factory, IRepository<UserStreamingConnection, Guid> userStreamingConnectionRepository, IOAuthStateProvider stateProvider)
    {
        _factory = factory;
        _userStreamingConnectionRepository = userStreamingConnectionRepository;
        _stateProvider = stateProvider;
    }
    public string GetAuthorizationUrl(StreamingService service, Guid userId)
    {
        var state = _stateProvider.Protect(userId);
        return _factory.GetStrategy(service).GetAuthorizationUrl(state);
    }

    public async Task HandleCallbackAsync(string state, StreamingService service, string code)
    {
        var strategy = _factory.GetStrategy(service);
        var userId = _stateProvider.Unprotect(state);

        var token = await strategy.ExchangeCodeAsync(code);
        var expiresAt = DateTimeOffset.UtcNow.AddSeconds(token.ExpiresIn);

        var connection = await _userStreamingConnectionRepository.All
            .FirstOrDefaultAsync(x => x.UserId == userId && x.Service == service);

        if (connection is null)
        {
            connection = new UserStreamingConnection
            {
                UserId = userId,
                Service = service,
                AccessToken = token.AccessToken,
                RefreshToken = token.RefreshToken,
                AccessTokenExpiresAtUtc = expiresAt
            };
            await _userStreamingConnectionRepository.CreateAsync(connection);
            return;
        }

        connection.AccessToken = token.AccessToken;
        connection.RefreshToken = token.RefreshToken;
        connection.AccessTokenExpiresAtUtc = expiresAt;

        await _userStreamingConnectionRepository.UpdateAsync(connection);
    }
}
