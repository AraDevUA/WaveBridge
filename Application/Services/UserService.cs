using Application.Dto.DtoExtensions;
using Application.Dto.Request.Users;
using Application.Dto.Response.Users;
using Application.Providers;
using Application.Providers.Contracts;
using Application.Results;
using Application.Results.Interfaces;
using Application.Services.Contracts;
using Domain.Entities;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IRepository<User, Guid> _userRepository;
        private readonly IJwtProvider _jwtProvider;
        public UserService(UserManager<User> userManager, IRepository<User, Guid> userRepository, IJwtProvider jwtProvider)
        {
            _userManager = userManager;
            _userRepository=userRepository;
            _jwtProvider = jwtProvider;
        }
        public async Task<IServiceResult> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user is null)
                return ServiceResults.NotFound();

            return ServiceResults.Ok(user);
        }
        public async Task<IServiceResult> GetAllAsync(CancellationToken cancellationToken = default)
        {
            //TODO: paged
            var users = await _userRepository.All
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var result = users.Select(u => u.ToDto()).ToList();

            return ServiceResults.Ok(result);
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
    }
}
