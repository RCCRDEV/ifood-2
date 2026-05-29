using FoodDelivery.Models.Enums;

namespace FoodDelivery.DTOs;

public sealed record PedidoDto(
    Guid Id,
    string Codigo,
    DateTime DataPedidoUtc,
    string RestauranteNome,
    string ClienteNome,
    string? ClienteTelefone,
    string? ClienteEndereco,
    string? MotoboyNome,
    PedidoStatus Status,
    string StatusLabel,
    MetodoPagamento MetodoPagamento,
    StatusPagamento StatusPagamento,
    string PagamentoLabel,
    string? Observacoes,
    string? CancelamentoMotivo,
    bool HasCancelamentoMotivo,
    bool CanCancel,
    decimal Total,
    IReadOnlyList<PedidoItemDto> Itens
);
