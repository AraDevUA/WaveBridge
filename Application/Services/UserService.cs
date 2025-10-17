using Application.Results;
using Application.Results.Interfaces;
using Application.Services.Contracts;
using Domain.Entities;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<User, Guid> _userRepository;
        public UserService(IRepository<User, Guid> userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<IServiceResult> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var result = await _userRepository.FindAsync(cancellationToken, id);
            return ServiceResults.Ok(result);
        }
        public async Task<IServiceResult> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var users = await _userRepository.All
                .AsNoTracking()
                .ToListAsync();

            if (users is null)
                return ServiceResults.NotFound();

            return ServiceResults.Ok(users);
        }
    }
}
