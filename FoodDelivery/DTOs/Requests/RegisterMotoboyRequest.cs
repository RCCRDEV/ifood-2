namespace FoodDelivery.DTOs.Requests;

public sealed record RegisterMotoboyRequest(
    string Nome,
    string Email,
    string Password,
    string? PlacaVeiculo
);

