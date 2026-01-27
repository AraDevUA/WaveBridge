using Application.Dto.Request.Auth;
using Application.Dto.Request.Users;
using Application.Dto.Response.Users;
using Application.Dto.Responses.Auth;
using Application.Dto.Responses.Auth.Google;
using Domain.Entities;
using Domain.Enums;

namespace Application.Dto.DtoExtensions;

public static class UserDtoExtensions
{
    public static User ToEntity(this RegisterRequestDto dto)
    {
        return new User
        {
            UserName = dto.UserName,
            Email = dto.Email,
        };
    }
    public static User ToUserEntity(this GoogleUserInfoResponseDto dto)
    {
        return new User
        {
            UserName = dto.Email.Split('@')[0],
            Email = dto.Email,
            EmailConfirmed = true
        };
    }
    public static UserOAuthConnection ToEntity(this GoogleUserInfoResponseDto googleUser, GoogleTokenResponseDto tokenDto, Guid userId)
    {
        return new UserOAuthConnection
        {
            UserId = userId,
            ProviderUserId = googleUser.Id,
            AccessToken = tokenDto.AccessToken,
            RefreshToken = tokenDto.RefreshToken,
            AccessTokenExpiresAtUtc = DateTimeOffset.UtcNow.AddSeconds(tokenDto.ExpiresIn)
        };
    }
    public static void UpdateEntity(this UserUpdateDto dto, User user)
    {
        if (!string.IsNullOrWhiteSpace(dto.UserName))
            user.UserName = dto.UserName;
        if (!string.IsNullOrWhiteSpace(dto.Email))
            user.Email = dto.Email;
    }
    public static UserResponseDto ToDto(this User user, IEnumerable<string>? roles = null)
    {
        return new UserResponseDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            Roles = roles?.ToList() ?? new List<string>()
        };
    }
    public static PageDto<UserResponseDto> ToPageDto(this IEnumerable<User> users, int totalCount, IEnumerable<string>? roles = null)
    {
        return new PageDto<UserResponseDto>
        {
            Items = users.Select(u => u.ToDto(roles)).ToList(),
            TotalCount = totalCount
        };
    }
}
