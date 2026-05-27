using FoodDelivery.Data;
using FoodDelivery.Models.Users;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly FoodDeliveryDbContext _db;

    public UserRepository(FoodDeliveryDbContext db)
    {
        _db = db;
    }

    public Task<AppUser?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        return _db.Users.FirstOrDefaultAsync(u => u.Email == email, ct);
    }

    public Task<AppUser?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return _db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
    }

    public async Task<List<AppUser>> ListAsync(CancellationToken ct = default)
    {
        return await _db.Users.OrderBy(u => u.Nome).ToListAsync(ct);
    }

    public async Task AddAsync(AppUser user, CancellationToken ct = default)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(AppUser user, CancellationToken ct = default)
    {
        _db.Users.Update(user);
        await _db.SaveChangesAsync(ct);
    }
}

