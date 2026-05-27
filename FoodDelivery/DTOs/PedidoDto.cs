using FoodDelivery.Models.Enums;

namespace FoodDelivery.DTOs;

public sealed record PedidoDto(
    Guid Id,
    DateTime DataPedidoUtc,
    string RestauranteNome,
    string ClienteNome,
    string? MotoboyNome,
    PedidoStatus Status,
    decimal Total,
    IReadOnlyList<PedidoItemDto> Itens
);

