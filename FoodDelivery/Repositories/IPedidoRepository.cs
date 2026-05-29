using FoodDelivery.Models;
using FoodDelivery.Models.Enums;

namespace FoodDelivery.Repositories;

public interface IPedidoRepository
{
    Task<Pedido?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<Pedido>> ListByClienteAsync(Guid clienteId, CancellationToken ct = default);
    Task<List<Pedido>> ListByRestauranteAsync(Guid restauranteId, CancellationToken ct = default);
    Task<List<Pedido>> ListByMotoboyAsync(Guid motoboyId, CancellationToken ct = default);
    Task<List<Pedido>> ListDisponiveisEntregaAsync(CancellationToken ct = default);
    Task<List<Pedido>> ListAllAsync(CancellationToken ct = default);
    Task AddAsync(Pedido pedido, CancellationToken ct = default);
    Task UpdateStatusAsync(Guid pedidoId, PedidoStatus status, CancellationToken ct = default);
    Task CancelAsync(Guid pedidoId, string motivo, CancellationToken ct = default);
    Task AssignMotoboyAsync(Guid pedidoId, Guid motoboyId, CancellationToken ct = default);
}
