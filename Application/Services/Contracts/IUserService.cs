using Application.Results.Interfaces;

namespace Application.Services.Contracts;

public interface IUserService
{
    Task<IServiceResult> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IServiceResult> GetAllAsync(CancellationToken cancellationToken = default);
}
