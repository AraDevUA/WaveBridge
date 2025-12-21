using Application.Dto.DtoExtensions;
using Application.Dto.Extensions;
using Application.Dto.Request.Users;
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
        private readonly IJwtProvider _jwtProvider;
        private readonly RoleManager<Role> _roleManager;
        public UserService(UserManager<User> userManager, IRepository<User, Guid> userRepository, IJwtProvider jwtProvider, RoleManager<Role> roleManager)
        {
            _userManager = userManager;
            _userRepository=userRepository;
            _jwtProvider = jwtProvider;
            _roleManager = roleManager;
        }
        public async Task<IServiceResult> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user is null)
                return ServiceResults.NotFound();

            return ServiceResults.Ok(user);
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

        public async Task<IServiceResult> UpdateAsync(Guid id, UserUpdateDto dto)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user is null)
                return ServiceResults.NotFound();

            dto.UpdateEntity(user);

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return ServiceResults.BadRequest(
                    string.Join("; ", result.Errors.Select(e => e.Description))
                );

            var roles = await _userManager.GetRolesAsync(user);
            return ServiceResults.Ok(user.ToDto(roles));
        }
        public async Task<IServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.FindAsync(cancellationToken, id);

            if (user is null)
                return ServiceResults.NotFound();
            await _userRepository.DeleteAsync(user);

            return ServiceResults.NoContent();
        }
        public async Task<IServiceResult> AssignRoleAsync(Guid userId, Guid roleId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null)
                return ServiceResults.NotFound();

            var role = await _roleManager.FindByIdAsync(roleId.ToString());

            if (role is null)
                return ServiceResults.NotFound();

            //TODO: Improve error handling 
            var result = await _userManager.AddToRoleAsync(user, role.Name);
            if (!result.Succeeded)
                return ServiceResults.Failed(string.Join("; ", result.Errors.Select(e => e.Description)));

            var roles = await _userManager.GetRolesAsync(user);

            return ServiceResults.Ok(user.ToDto(roles));
        }
        public async Task<IServiceResult> GetRolesAsync()
        {
            var roles = _roleManager.Roles
                .Select(r => new { r.Id, r.Name, r.CreatedUtc })
                .ToList();
            if(roles is null)
                return ServiceResults.NotFound();

            return ServiceResults.Ok(roles);
        }

    }
}
