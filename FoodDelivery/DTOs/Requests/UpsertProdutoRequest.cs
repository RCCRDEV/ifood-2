using FoodDelivery.Models.Enums;

namespace FoodDelivery.DTOs.Requests;

public sealed record UpsertProdutoRequest(
    Guid? Id,
    Guid RestauranteId,
    TipoProduto Tipo,
    string Nome,
    string? Descricao,
    decimal Preco,
    bool Ativo,
    int? TempoPreparoMin,
    string? ObservacoesPreparo,
    int? VolumeMl,
    bool? Alcoolica
);

