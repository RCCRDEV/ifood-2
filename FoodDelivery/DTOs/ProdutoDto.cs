using FoodDelivery.Models.Enums;

namespace FoodDelivery.DTOs;

public sealed record ProdutoDto(
    Guid Id,
    Guid RestauranteId,
    string Nome,
    string? Descricao,
    decimal Preco,
    bool Ativo,
    TipoProduto Tipo,
    int? TempoPreparoMin,
    int? VolumeMl,
    bool? Alcoolica
);

