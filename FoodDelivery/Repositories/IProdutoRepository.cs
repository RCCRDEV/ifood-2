using FoodDelivery.Models;

namespace FoodDelivery.Repositories;

public interface IProdutoRepository
{
    Task<Produto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<Produto>> ListByRestauranteAsync(Guid restauranteId, CancellationToken ct = default);
    Task AddAsync(Produto produto, CancellationToken ct = default);
    Task UpdateAsync(Produto produto, CancellationToken ct = default);
    Task DeleteAsync(Produto produto, CancellationToken ct = default);
}

