using Domain.Enums;

namespace Domain.Entities;

public class UserStreamingConnection
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public StreamingServices Service { get; set; }
    public string ExternalUserId { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTimeOffset AccessTokenExpiresAtUtc { get; set; }
    public virtual User User { get; set; }
}
