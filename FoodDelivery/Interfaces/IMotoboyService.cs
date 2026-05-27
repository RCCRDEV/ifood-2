using FoodDelivery.DTOs;
using FoodDelivery.Models.Enums;

namespace FoodDelivery.Services;

public interface IMotoboyService
{
    Task<List<PedidoDto>> ListEntregasDisponiveisAsync(CancellationToken ct = default);
    Task AceitarEntregaAsync(Guid pedidoId, Guid motoboyId, CancellationToken ct = default);
    Task AtualizarStatusEntregaAsync(Guid pedidoId, PedidoStatus status, CancellationToken ct = default);
    Task<List<PedidoDto>> HistoricoAsync(Guid motoboyId, CancellationToken ct = default);
}

