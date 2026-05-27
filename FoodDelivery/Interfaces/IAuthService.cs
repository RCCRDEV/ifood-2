using FoodDelivery.DTOs.Requests;
using FoodDelivery.Models.Users;

namespace FoodDelivery.Services;

public interface IAuthService
{
    Task<AppUser> LoginAsync(string email, string password, CancellationToken ct = default);
    Task<Cliente> RegisterClienteAsync(RegisterClienteRequest request, CancellationToken ct = default);
    Task<Motoboy> RegisterMotoboyAsync(RegisterMotoboyRequest request, CancellationToken ct = default);
    Task<RestauranteUser> RegisterRestauranteAsync(RegisterRestauranteRequest request, CancellationToken ct = default);
    Task UpdateClientePerfilAsync(Guid clienteId, string nome, string? telefone, string? endereco, CancellationToken ct = default);
}

