using Application.Dto.Request.Auth;
using Application.Dto.Request.Users;
using Application.Results.Interfaces;

namespace Application.Services.Contracts;

public interface IUserService
{
    Task<IServiceResult> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IServiceResult> GetAllAsync(UserRequestDto dto, CancellationToken cancellationToken = default);
    Task<IServiceResult> UpdateAsync(Guid id, UserUpdateDto dto);
    Task<IServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IServiceResult> AssignRoleAsync(Guid userId, Guid roleId);
    Task<IServiceResult> GetRolesAsync();
}
