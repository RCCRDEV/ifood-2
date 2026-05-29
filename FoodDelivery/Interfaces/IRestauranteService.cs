using FoodDelivery.DTOs;
using FoodDelivery.DTOs.Requests;
using FoodDelivery.Models.Enums;

namespace FoodDelivery.Services;

public interface IRestauranteService
{
    Task<List<ProdutoDto>> ListProdutosAsync(Guid restauranteId, CancellationToken ct = default);
    Task<ProdutoDto> UpsertProdutoAsync(UpsertProdutoRequest request, CancellationToken ct = default);
    Task ExcluirProdutoAsync(Guid produtoId, CancellationToken ct = default);
    Task<List<PedidoDto>> ListPedidosAsync(Guid restauranteId, CancellationToken ct = default);
    Task AtualizarStatusAsync(Guid pedidoId, PedidoStatus status, CancellationToken ct = default);
    Task ConfirmarPedidoAsync(Guid pedidoId, CancellationToken ct = default);
    Task RecusarPedidoAsync(Guid pedidoId, string motivo, CancellationToken ct = default);
}
