namespace FoodDelivery.DTOs.Requests;

public sealed record RegisterRestauranteRequest(
    string NomeUsuario,
    string Email,
    string Password,
    string NomeRestaurante,
    string? Descricao,
    string? Endereco,
    string? Telefone
);

