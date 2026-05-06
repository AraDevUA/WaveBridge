using Application.Dto.DtoExtensions;
using Application.Dto.Request.Users;
using Application.Dto.Response.Users;
using Application.Extensions;
using Application.Providers.Contracts;
using Application.Results;
using Application.Results.Interfaces;
using Application.Services.Contracts;
using Domain.Entities;
using Infrastructure.Repositories.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IRepository<User, Guid> _userRepository;
        private readonly RoleManager<Role> _roleManager;

        public UserService(
            UserManager<User> userManager,
            IRepository<User, Guid> userRepository,
            RoleManager<Role> roleManager)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _roleManager = roleManager;
        }
        public async Task<IServiceResult> GetCurrentProfileAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null)
                return ServiceResults.NotFound();

            return ServiceResults.Ok(new ProfileResponseDto
            {
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                AvatarUrl = user.AvatarUrl
            });
        }
        public async Task<IServiceResult> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user is null)
                return ServiceResults.NotFound();
            
            return ServiceResults.Ok(user.ToDto());
        }
        public async Task<IServiceResult> GetAllAsync(UserRequestDto dto, CancellationToken cancellationToken = default)
        {

            var users = _userRepository.All.AsNoTracking()
                .WhereIf(!string.IsNullOrWhiteSpace(dto.UserName), u => u.UserName.Contains(dto.UserName))
                .WhereIf(!string.IsNullOrWhiteSpace(dto.Email), u => u.Email.Contains(dto.Email));

            var totalCount = await users.CountAsync(cancellationToken);
            
            var result = await users
                .Paginate(dto.Page, dto.PageSize)
                .ToListAsync(cancellationToken);
            
            return ServiceResults.Ok(result.ToPageDto(totalCount));
        }

        public async Task<IServiceResult> UpdateAsync(Guid id, UserUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user is null)
                return ServiceResults.NotFound();

            dto.UpdateEntity(user);

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return ServiceResults.Failed(result.Errors.ToValidationErrors());

            var roles = await _userManager.GetRolesAsync(user);
            return ServiceResults.Ok(user.ToDto(roles));
        }
        public async Task<IServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.FindAsync(cancellationToken, id);

            if (user is null)
                return ServiceResults.NotFound();

            await _userRepository.DeleteAsync(user, cancellationToken);

            return ServiceResults.NoContent();
        }
        public async Task<IServiceResult> AssignRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null)
                return ServiceResults.NotFound();

            var role = await _roleManager.FindByIdAsync(roleId.ToString());

            if (role is null)
                return ServiceResults.NotFound();

            var result = await _userManager.AddToRoleAsync(user, role.Name);
            if (!result.Succeeded)
                return ServiceResults.Failed(result.Errors.ToValidationErrors());

            var roles = await _userManager.GetRolesAsync(user);

            return ServiceResults.Ok(user.ToDto(roles));
        }
        public async Task<IServiceResult> GetRolesAsync(CancellationToken cancellationToken = default)
        {
            var roles = await _roleManager.Roles
                .AsNoTracking()
                .Select(r => new { r.Id, r.Name, r.CreatedUtc })
                .ToListAsync(cancellationToken);

            return ServiceResults.Ok(roles);
        }

    }
}
