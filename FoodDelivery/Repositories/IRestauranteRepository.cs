using FoodDelivery.Models;

namespace FoodDelivery.Repositories;

public interface IRestauranteRepository
{
    Task<Restaurante?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<Restaurante>> ListAtivosAsync(CancellationToken ct = default);
    Task<List<Restaurante>> ListAsync(CancellationToken ct = default);
    Task AddAsync(Restaurante restaurante, CancellationToken ct = default);
    Task UpdateAsync(Restaurante restaurante, CancellationToken ct = default);
}

