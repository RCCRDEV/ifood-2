using FoodDelivery.Data;
using FoodDelivery.DTOs.Requests;
using FoodDelivery.Helpers;
using FoodDelivery.Models;
using FoodDelivery.Models.Users;
using FoodDelivery.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Services;

public sealed class AuthService : IAuthService
{
    private readonly FoodDeliveryDbContext _db;
    private readonly IUserRepository _users;
    private readonly IRestauranteRepository _restaurantes;
    private readonly PasswordHasher _hasher;

    public AuthService(
        FoodDeliveryDbContext db,
        IUserRepository users,
        IRestauranteRepository restaurantes,
        PasswordHasher hasher)
    {
        _db = db;
        _users = users;
        _restaurantes = restaurantes;
        _hasher = hasher;
    }

    public async Task<AppUser> LoginAsync(string email, string password, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            throw new FriendlyException("Informe e-mail e senha.");

        var user = await _users.GetByEmailAsync(email.Trim().ToLowerInvariant(), ct);
        if (user is null || !user.Ativo)
            throw new FriendlyException("Usuário ou senha inválidos.");

        if (!_hasher.Verify(password, user.PasswordHash, user.PasswordSalt))
            throw new FriendlyException("Usuário ou senha inválidos.");

        return user;
    }

    public async Task<Cliente> RegisterClienteAsync(RegisterClienteRequest request, CancellationToken ct = default)
    {
        ValidateBasicUser(request.Nome, request.Email, request.Password);

        var email = request.Email.Trim().ToLowerInvariant();
        if (await _users.GetByEmailAsync(email, ct) is not null)
            throw new FriendlyException("Este e-mail já está cadastrado.");

        var (hash, salt) = _hasher.Hash(request.Password);
        var cliente = new Cliente
        {
            Nome = request.Nome.Trim(),
            Email = email,
            PasswordHash = hash,
            PasswordSalt = salt,
            Telefone = request.Telefone?.Trim(),
            Endereco = request.Endereco?.Trim()
        };

        await _users.AddAsync(cliente, ct);
        return cliente;
    }

    public async Task<Motoboy> RegisterMotoboyAsync(RegisterMotoboyRequest request, CancellationToken ct = default)
    {
        ValidateBasicUser(request.Nome, request.Email, request.Password);

        var email = request.Email.Trim().ToLowerInvariant();
        if (await _users.GetByEmailAsync(email, ct) is not null)
            throw new FriendlyException("Este e-mail já está cadastrado.");

        var (hash, salt) = _hasher.Hash(request.Password);
        var motoboy = new Motoboy
        {
            Nome = request.Nome.Trim(),
            Email = email,
            PasswordHash = hash,
            PasswordSalt = salt,
            PlacaVeiculo = request.PlacaVeiculo?.Trim()
        };

        await _users.AddAsync(motoboy, ct);
        return motoboy;
    }

    public async Task<RestauranteUser> RegisterRestauranteAsync(RegisterRestauranteRequest request, CancellationToken ct = default)
    {
        ValidateBasicUser(request.NomeUsuario, request.Email, request.Password);

        if (string.IsNullOrWhiteSpace(request.NomeRestaurante))
            throw new FriendlyException("Informe o nome do restaurante.");

        var email = request.Email.Trim().ToLowerInvariant();
        if (await _users.GetByEmailAsync(email, ct) is not null)
            throw new FriendlyException("Este e-mail já está cadastrado.");

        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        var restaurante = new Restaurante
        {
            Nome = request.NomeRestaurante.Trim(),
            Descricao = request.Descricao?.Trim(),
            Endereco = request.Endereco?.Trim(),
            Telefone = request.Telefone?.Trim()
        };

        await _restaurantes.AddAsync(restaurante, ct);

        var (hash, salt) = _hasher.Hash(request.Password);
        var user = new RestauranteUser
        {
            Nome = request.NomeUsuario.Trim(),
            Email = email,
            PasswordHash = hash,
            PasswordSalt = salt,
            RestauranteId = restaurante.Id,
            Cargo = "Dono"
        };

        await _users.AddAsync(user, ct);

        await tx.CommitAsync(ct);
        return user;
    }

    public async Task UpdateClientePerfilAsync(Guid clienteId, string nome, string? telefone, string? endereco, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new FriendlyException("Informe o nome.");

        var cliente = await _db.Clientes.FirstOrDefaultAsync(c => c.Id == clienteId, ct);
        if (cliente is null)
            throw new FriendlyException("Cliente não encontrado.");

        cliente.Nome = nome.Trim();
        cliente.Telefone = telefone?.Trim();
        cliente.Endereco = endereco?.Trim();
        await _db.SaveChangesAsync(ct);
    }

    private static void ValidateBasicUser(string nome, string email, string password)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new FriendlyException("Informe seu nome.");

        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new FriendlyException("Informe um e-mail válido.");

        if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            throw new FriendlyException("A senha deve ter no mínimo 6 caracteres.");
    }
}

