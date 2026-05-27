namespace FoodDelivery.DTOs;

public sealed record UserDto(
    Guid Id,
    string Nome,
    string Email,
    string Tipo,
    bool Ativo
);
