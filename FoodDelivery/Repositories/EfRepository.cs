using FoodDelivery.Data;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Repositories;

public sealed class EfRepository<T> : IRepository<T> where T : class
{
    private readonly FoodDeliveryDbContext _db;

    public EfRepository(FoodDeliveryDbContext db)
    {
        _db = db;
    }

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _db.Set<T>().FindAsync([id], ct);
    }

    public async Task<List<T>> ListAsync(CancellationToken ct = default)
    {
        return await _db.Set<T>().ToListAsync(ct);
    }

    public async Task AddAsync(T entity, CancellationToken ct = default)
    {
        _db.Set<T>().Add(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(T entity, CancellationToken ct = default)
    {
        _db.Set<T>().Update(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(T entity, CancellationToken ct = default)
    {
        _db.Set<T>().Remove(entity);
        await _db.SaveChangesAsync(ct);
    }
}

