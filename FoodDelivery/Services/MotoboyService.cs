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
        return list
            .Where(p => p.Status != PedidoStatus.Cancelado)
            .Select(p => p.ToDto())
            .ToList();
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
        await _pedidos.UpdateStatusAsync(pedidoId, PedidoStatus.EmEntrega, ct);
    }

    public async Task MarcarComoEntregueAsync(Guid pedidoId, Guid motoboyId, CancellationToken ct = default)
    {
        if (pedidoId == Guid.Empty || motoboyId == Guid.Empty)
            throw new FriendlyException("Entrega inválida.");

        var pedido = await _pedidos.GetByIdAsync(pedidoId, ct);
        if (pedido is null)
            throw new FriendlyException("Pedido não encontrado.");

        if (pedido.MotoboyId is null)
            throw new FriendlyException("Entrega sem motoboy.");

        if (pedido.MotoboyId != motoboyId)
            throw new FriendlyException("Esta entrega pertence a outro motoboy.");

        if (pedido.Status != PedidoStatus.EmEntrega)
            throw new FriendlyException("Só é possível marcar como entregue quando a entrega está em andamento.");

        await _pedidos.UpdateStatusAsync(pedidoId, PedidoStatus.Entregue, ct);
    }

    public async Task ReportarNaoEntregaAsync(Guid pedidoId, Guid motoboyId, string motivo, CancellationToken ct = default)
    {
        if (pedidoId == Guid.Empty || motoboyId == Guid.Empty)
            throw new FriendlyException("Entrega inválida.");

        var pedido = await _pedidos.GetByIdAsync(pedidoId, ct);
        if (pedido is null)
            throw new FriendlyException("Pedido não encontrado.");

        if (pedido.MotoboyId is null)
            throw new FriendlyException("Entrega sem motoboy.");

        if (pedido.MotoboyId != motoboyId)
            throw new FriendlyException("Esta entrega pertence a outro motoboy.");

        if (pedido.Status != PedidoStatus.EmEntrega)
            throw new FriendlyException("Só é possível reportar problema quando a entrega está em andamento.");

        var motivoFinal = string.IsNullOrWhiteSpace(motivo)
            ? "Entrega não concluída pelo motoboy."
            : motivo.Trim();

        await _pedidos.CancelAsync(pedidoId, motivoFinal, ct);
    }

    public async Task<List<PedidoDto>> HistoricoAsync(Guid motoboyId, CancellationToken ct = default)
    {
        var list = await _pedidos.ListByMotoboyAsync(motoboyId, ct);
        return list.Select(p => p.ToDto()).ToList();
    }
}
