using FoodDelivery.Data;
using FoodDelivery.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Repositories;

public sealed class RestauranteRepository : IRestauranteRepository
{
    private readonly FoodDeliveryDbContext _db;

    public RestauranteRepository(FoodDeliveryDbContext db)
    {
        _db = db;
    }

    public Task<Restaurante?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return _db.Restaurantes
            .Include(r => r.Produtos.Where(p => p.Ativo))
            .FirstOrDefaultAsync(r => r.Id == id, ct);
    }

    public Task<List<Restaurante>> ListAtivosAsync(CancellationToken ct = default)
    {
        return _db.Restaurantes
            .Where(r => r.Ativo)
            .OrderBy(r => r.Nome)
            .ToListAsync(ct);
    }

    public Task<List<Restaurante>> ListAsync(CancellationToken ct = default)
    {
        return _db.Restaurantes.OrderBy(r => r.Nome).ToListAsync(ct);
    }

    public async Task AddAsync(Restaurante restaurante, CancellationToken ct = default)
    {
        _db.Restaurantes.Add(restaurante);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Restaurante restaurante, CancellationToken ct = default)
    {
        _db.Restaurantes.Update(restaurante);
        await _db.SaveChangesAsync(ct);
    }
}

