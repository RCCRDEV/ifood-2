using FoodDelivery.DTOs;
using FoodDelivery.Helpers;
using FoodDelivery.Models.Enums;
using FoodDelivery.Repositories;

namespace FoodDelivery.Services;

public sealed class MotoboyService : IMotoboyService
{
    private readonly IPedidoRepository _pedidos;

    public MotoboyService(IPedidoRepository pedidos)
    {
        _pedidos = pedidos;
    }

    public async Task<List<PedidoDto>> ListEntregasDisponiveisAsync(CancellationToken ct = default)
    {
        var list = await _pedidos.ListDisponiveisEntregaAsync(ct);
        return list.Select(p => p.ToDto()).ToList();
    }

    public async Task AceitarEntregaAsync(Guid pedidoId, Guid motoboyId, CancellationToken ct = default)
    {
        if (pedidoId == Guid.Empty || motoboyId == Guid.Empty)
            throw new FriendlyException("Entrega inválida.");

        var pedido = await _pedidos.GetByIdAsync(pedidoId, ct);
        if (pedido is null)
            throw new FriendlyException("Pedido não encontrado.");

        if (pedido.Status != PedidoStatus.SaiuParaEntrega)
            throw new FriendlyException("Este pedido ainda não está disponível para entrega.");

        if (pedido.MotoboyId is not null)
            throw new FriendlyException("Esta entrega já foi aceita.");

        await _pedidos.AssignMotoboyAsync(pedidoId, motoboyId, ct);
    }

    public async Task AtualizarStatusEntregaAsync(Guid pedidoId, PedidoStatus status, CancellationToken ct = default)
    {
        if (status is not PedidoStatus.Entregue and not PedidoStatus.Cancelado)
            throw new FriendlyException("Status de entrega inválido.");

        await _pedidos.UpdateStatusAsync(pedidoId, status, ct);
    }

    public async Task<List<PedidoDto>> HistoricoAsync(Guid motoboyId, CancellationToken ct = default)
    {
        var list = await _pedidos.ListByMotoboyAsync(motoboyId, ct);
        return list.Select(p => p.ToDto()).ToList();
    }
}
