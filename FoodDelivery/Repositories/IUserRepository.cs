using FoodDelivery.Models.Users;

namespace FoodDelivery.Repositories;

public interface IUserRepository
{
    Task<AppUser?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<AppUser?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<AppUser>> ListAsync(CancellationToken ct = default);
    Task AddAsync(AppUser user, CancellationToken ct = default);
    Task UpdateAsync(AppUser user, CancellationToken ct = default);
}

