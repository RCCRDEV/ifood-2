using FoodDelivery.Data;
using FoodDelivery.DTOs;
using FoodDelivery.Helpers;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Services;

public sealed class AdminService : IAdminService
{
    private readonly FoodDeliveryDbContext _db;

    public AdminService(FoodDeliveryDbContext db)
    {
        _db = db;
    }

    public async Task<DashboardDto> GetDashboardAsync(CancellationToken ct = default)
    {
        var totalUsuarios = await _db.Users.CountAsync(ct);
        var totalRestaurantes = await _db.Restaurantes.CountAsync(ct);
        var totalPedidos = await _db.Pedidos.CountAsync(ct);

        var hoje = DateTime.UtcNow.Date;
        var pedidosHoje = await _db.Pedidos.CountAsync(p => p.DataPedidoUtc >= hoje, ct);

        return new DashboardDto(totalUsuarios, totalRestaurantes, totalPedidos, pedidosHoje);
    }

    public async Task<List<UserDto>> ListUsuariosAsync(CancellationToken ct = default)
    {
        var users = await _db.Users.AsNoTracking().OrderBy(u => u.Nome).ToListAsync(ct);
        return users.Select(u => u.ToDto()).ToList();
    }

    public async Task ToggleUsuarioAtivoAsync(Guid userId, bool ativo, CancellationToken ct = default)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);
        if (user is null) return;
        user.Ativo = ativo;
        await _db.SaveChangesAsync(ct);
    }

    public async Task<List<RestauranteDto>> ListRestaurantesAsync(CancellationToken ct = default)
    {
        var list = await _db.Restaurantes.AsNoTracking().OrderBy(r => r.Nome).ToListAsync(ct);
        return list.Select(r => r.ToDto()).ToList();
    }

    public async Task ToggleRestauranteAtivoAsync(Guid restauranteId, bool ativo, CancellationToken ct = default)
    {
        var restaurante = await _db.Restaurantes.FirstOrDefaultAsync(r => r.Id == restauranteId, ct);
        if (restaurante is null) return;
        restaurante.Ativo = ativo;
        await _db.SaveChangesAsync(ct);
    }

    public async Task<List<PedidoDto>> ListPedidosAsync(CancellationToken ct = default)
    {
        var pedidos = await _db.Pedidos
            .AsNoTracking()
            .Include(p => p.Restaurante)
            .Include(p => p.Cliente)
            .Include(p => p.Motoboy)
            .Include(p => p.Itens)
                .ThenInclude(i => i.Produto)
            .OrderByDescending(p => p.DataPedidoUtc)
            .ToListAsync(ct);

        return pedidos.Select(p => p.ToDto()).ToList();
    }
}

