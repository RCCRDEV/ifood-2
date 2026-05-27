namespace FoodDelivery.DTOs.Requests;

public sealed record RegisterClienteRequest(
    string Nome,
    string Email,
    string Password,
    string? Telefone,
    string? Endereco
);

