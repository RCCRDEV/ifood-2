using FoodDelivery.DTOs;

namespace FoodDelivery.Services;

public interface IAdminService
{
    Task<DashboardDto> GetDashboardAsync(CancellationToken ct = default);
    Task<List<UserDto>> ListUsuariosAsync(CancellationToken ct = default);
    Task ToggleUsuarioAtivoAsync(Guid userId, bool ativo, CancellationToken ct = default);
    Task<List<RestauranteDto>> ListRestaurantesAsync(CancellationToken ct = default);
    Task ToggleRestauranteAtivoAsync(Guid restauranteId, bool ativo, CancellationToken ct = default);
    Task<List<PedidoDto>> ListPedidosAsync(CancellationToken ct = default);
}

