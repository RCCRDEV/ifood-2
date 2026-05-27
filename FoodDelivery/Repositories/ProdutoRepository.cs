using FoodDelivery.Data;
using FoodDelivery.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Repositories;

public sealed class ProdutoRepository : IProdutoRepository
{
    private readonly FoodDeliveryDbContext _db;

    public ProdutoRepository(FoodDeliveryDbContext db)
    {
        _db = db;
    }

    public Task<Produto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return _db.Produtos.FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public Task<List<Produto>> ListByRestauranteAsync(Guid restauranteId, CancellationToken ct = default)
    {
        return _db.Produtos
            .Where(p => p.RestauranteId == restauranteId)
            .OrderByDescending(p => p.Ativo)
            .ThenBy(p => p.Nome)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Produto produto, CancellationToken ct = default)
    {
        _db.Produtos.Add(produto);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Produto produto, CancellationToken ct = default)
    {
        _db.Produtos.Update(produto);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Produto produto, CancellationToken ct = default)
    {
        _db.Produtos.Remove(produto);
        await _db.SaveChangesAsync(ct);
    }
}

