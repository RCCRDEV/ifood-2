namespace FoodDelivery.DTOs;

public sealed record PedidoItemDto(
    Guid ProdutoId,
    string ProdutoNome,
    int Quantidade,
    decimal PrecoUnitario,
    decimal Subtotal
);

