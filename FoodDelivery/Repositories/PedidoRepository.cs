using FoodDelivery.Data;
using FoodDelivery.Models;
using FoodDelivery.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Repositories;

public sealed class PedidoRepository : IPedidoRepository
{
    private readonly FoodDeliveryDbContext _db;

    public PedidoRepository(FoodDeliveryDbContext db)
    {
        _db = db;
    }

    public Task<Pedido?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return Query()
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public Task<List<Pedido>> ListByClienteAsync(Guid clienteId, CancellationToken ct = default)
    {
        return Query()
            .Where(p => p.ClienteId == clienteId)
            .OrderByDescending(p => p.DataPedidoUtc)
            .ToListAsync(ct);
    }

    public Task<List<Pedido>> ListByRestauranteAsync(Guid restauranteId, CancellationToken ct = default)
    {
        return Query()
            .Where(p => p.RestauranteId == restauranteId)
            .OrderByDescending(p => p.DataPedidoUtc)
            .ToListAsync(ct);
    }

    public Task<List<Pedido>> ListByMotoboyAsync(Guid motoboyId, CancellationToken ct = default)
    {
        return Query()
            .Where(p => p.MotoboyId == motoboyId)
            .OrderByDescending(p => p.DataPedidoUtc)
            .ToListAsync(ct);
    }

    public Task<List<Pedido>> ListDisponiveisEntregaAsync(CancellationToken ct = default)
    {
        return Query()
            .Where(p => p.Status == PedidoStatus.SaiuParaEntrega && p.MotoboyId == null)
            .OrderBy(p => p.DataPedidoUtc)
            .ToListAsync(ct);
    }

    public Task<List<Pedido>> ListAllAsync(CancellationToken ct = default)
    {
        return Query()
            .OrderByDescending(p => p.DataPedidoUtc)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Pedido pedido, CancellationToken ct = default)
    {
        _db.Pedidos.Add(pedido);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateStatusAsync(Guid pedidoId, PedidoStatus status, CancellationToken ct = default)
    {
        var pedido = await _db.Pedidos.FirstOrDefaultAsync(p => p.Id == pedidoId, ct);
        if (pedido is null) return;
        pedido.Status = status;
        await _db.SaveChangesAsync(ct);
    }

    public async Task CancelAsync(Guid pedidoId, string motivo, CancellationToken ct = default)
    {
        var pedido = await _db.Pedidos.FirstOrDefaultAsync(p => p.Id == pedidoId, ct);
        if (pedido is null) return;
        pedido.Status = PedidoStatus.Cancelado;
        pedido.CancelamentoMotivo = string.IsNullOrWhiteSpace(motivo) ? "Cancelado." : motivo.Trim();
        if (pedido.MetodoPagamento != MetodoPagamento.Dinheiro && pedido.StatusPagamento == StatusPagamento.Aprovado)
            pedido.StatusPagamento = StatusPagamento.Estornado;
        await _db.SaveChangesAsync(ct);
    }

    public async Task AssignMotoboyAsync(Guid pedidoId, Guid motoboyId, CancellationToken ct = default)
    {
        var pedido = await _db.Pedidos.FirstOrDefaultAsync(p => p.Id == pedidoId, ct);
        if (pedido is null) return;
        pedido.MotoboyId = motoboyId;
        await _db.SaveChangesAsync(ct);
    }

    private IQueryable<Pedido> Query()
    {
        return _db.Pedidos
            .AsNoTracking()
            .Include(p => p.Restaurante)
            .Include(p => p.Cliente)
            .Include(p => p.Motoboy)
            .Include(p => p.Itens)
                .ThenInclude(i => i.Produto);
    }
}
