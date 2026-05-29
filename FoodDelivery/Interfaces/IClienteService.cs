using FoodDelivery.DTOs;
using FoodDelivery.DTOs.Requests;

namespace FoodDelivery.Services;

public interface IClienteService
{
    Task<List<RestauranteDto>> ListRestaurantesAsync(CancellationToken ct = default);
    Task<List<ProdutoDto>> ListCardapioAsync(Guid restauranteId, CancellationToken ct = default);
    Task<PedidoDto> CriarPedidoAsync(CreatePedidoRequest request, CancellationToken ct = default);
    Task<List<PedidoDto>> ListPedidosAsync(Guid clienteId, CancellationToken ct = default);
    Task CancelarPedidoAsync(Guid clienteId, Guid pedidoId, string motivo, CancellationToken ct = default);
    Task FavoritarAsync(Guid clienteId, Guid restauranteId, CancellationToken ct = default);
    Task DesfavoritarAsync(Guid clienteId, Guid restauranteId, CancellationToken ct = default);
}
