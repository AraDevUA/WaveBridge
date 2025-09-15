using Domain.Enums;

namespace Domain.Entities;

public class UserConnection
{   public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public StreamingServices Service { get; set; }
    public string RefreshToken { get; set; }
    public string AccessToken { get; set; }
    public DateTime AccessTokenExpiresAt { get; set; }
    public User User { get; set; }
}
