using Application.Providers.Contracts;
using Application.Strateges.Abstractions;
using Application.Dto.Options;
using Application.Helpers;
using Domain.Entities;
using Shared.Enums;
using Infrastructure.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Application.Strateges.Auth;

public class StreamingAuthFacade : IStreamingAuthFacade
{
    private readonly IStreamingAuthFactory _factory;
    private readonly IRepository<UserStreamingConnection, Guid> _userStreamingConnectionRepository;
    private readonly IOAuthStateProvider _stateProvider;
    private readonly EncryptionOptions _encryptionOptions;
    public StreamingAuthFacade(
        IStreamingAuthFactory factory,
        IRepository<UserStreamingConnection, Guid> userStreamingConnectionRepository,
        IOAuthStateProvider stateProvider,
        IOptions<EncryptionOptions> encryptionOptions)
    {
        _factory = factory;
        _userStreamingConnectionRepository = userStreamingConnectionRepository;
        _stateProvider = stateProvider;
        _encryptionOptions = encryptionOptions.Value;
    }
    public string GetAuthorizationUrl(StreamingService service, Guid userId)
    {
        var state = _stateProvider.Protect(userId);
        return _factory.GetStrategy(service).GetAuthorizationUrl(state);
    }

    public async Task HandleCallbackAsync(string state, StreamingService service, string code, CancellationToken cancellationToken = default)
    {
        var strategy = _factory.GetStrategy(service);
        var userId = _stateProvider.Unprotect(state);

        var token = await strategy.ExchangeCodeAsync(code);
        var expiresAt = DateTimeOffset.UtcNow.AddSeconds(token.ExpiresIn);

        var connection = await _userStreamingConnectionRepository.All
            .FirstOrDefaultAsync(x => x.UserId == userId && x.Service == service, cancellationToken);

        if (connection is null)
        {
            connection = new UserStreamingConnection
            {
                UserId = userId,
                Service = service,
                AccessToken = token.AccessToken,
                RefreshToken = AesGcmHelper.Encrypt(token.RefreshToken, _encryptionOptions.KeyBase64),
                AccessTokenExpiresAtUtc = expiresAt
            };
            await _userStreamingConnectionRepository.CreateAsync(connection, cancellationToken);
            return;
        }

        connection.AccessToken = token.AccessToken;
        if (!string.IsNullOrEmpty(token.RefreshToken))
            connection.RefreshToken = AesGcmHelper.Encrypt(token.RefreshToken, _encryptionOptions.KeyBase64);

        connection.AccessTokenExpiresAtUtc = expiresAt;

        await _userStreamingConnectionRepository.UpdateAsync(connection, cancellationToken);
    }
}
