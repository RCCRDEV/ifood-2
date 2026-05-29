using FoodDelivery.Models.Enums;

namespace FoodDelivery.DTOs.Requests;

public sealed record CreatePedidoRequest(
    Guid ClienteId,
    Guid RestauranteId,
    MetodoPagamento MetodoPagamento,
    string? Observacoes,
    IReadOnlyList<CreatePedidoItemRequest> Itens
);

public sealed record CreatePedidoItemRequest(
    Guid ProdutoId,
    int Quantidade
);
