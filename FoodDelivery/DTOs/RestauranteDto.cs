namespace FoodDelivery.DTOs;

public sealed record RestauranteDto(
    Guid Id,
    string Nome,
    string? Descricao,
    string? Endereco,
    string? Telefone,
    bool Ativo
);
