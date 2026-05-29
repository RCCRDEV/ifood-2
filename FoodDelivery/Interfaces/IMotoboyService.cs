using FoodDelivery.DTOs;
using FoodDelivery.Models.Enums;

namespace FoodDelivery.Services;

public interface IMotoboyService
{
    Task<List<PedidoDto>> ListEntregasDisponiveisAsync(CancellationToken ct = default);
    Task AceitarEntregaAsync(Guid pedidoId, Guid motoboyId, CancellationToken ct = default);
    Task MarcarComoEntregueAsync(Guid pedidoId, Guid motoboyId, CancellationToken ct = default);
    Task ReportarNaoEntregaAsync(Guid pedidoId, Guid motoboyId, string motivo, CancellationToken ct = default);
    Task<List<PedidoDto>> HistoricoAsync(Guid motoboyId, CancellationToken ct = default);
}
