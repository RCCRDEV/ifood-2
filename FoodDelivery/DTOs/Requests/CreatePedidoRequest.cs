namespace FoodDelivery.DTOs.Requests;

public sealed record CreatePedidoRequest(
    Guid ClienteId,
    Guid RestauranteId,
    string? Observacoes,
    IReadOnlyList<CreatePedidoItemRequest> Itens
);

public sealed record CreatePedidoItemRequest(
    Guid ProdutoId,
    int Quantidade
);

