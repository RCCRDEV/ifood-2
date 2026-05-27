using FoodDelivery.Data;
using FoodDelivery.DTOs;
using FoodDelivery.DTOs.Requests;
using FoodDelivery.Helpers;
using FoodDelivery.Models;
using FoodDelivery.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Services;

public sealed class ClienteService : IClienteService
{
    private readonly FoodDeliveryDbContext _db;
    private readonly IRestauranteRepository _restaurantes;
    private readonly IProdutoRepository _produtos;
    private readonly IPedidoRepository _pedidos;

    public ClienteService(
        FoodDeliveryDbContext db,
        IRestauranteRepository restaurantes,
        IProdutoRepository produtos,
        IPedidoRepository pedidos)
    {
        _db = db;
        _restaurantes = restaurantes;
        _produtos = produtos;
        _pedidos = pedidos;
    }

    public async Task<List<RestauranteDto>> ListRestaurantesAsync(CancellationToken ct = default)
    {
        var list = await _restaurantes.ListAtivosAsync(ct);
        return list.Select(r => r.ToDto()).ToList();
    }

    public async Task<List<ProdutoDto>> ListCardapioAsync(Guid restauranteId, CancellationToken ct = default)
    {
        var produtos = await _produtos.ListByRestauranteAsync(restauranteId, ct);
        return produtos.Where(p => p.Ativo).Select(p => p.ToDto()).ToList();
    }

    public async Task<PedidoDto> CriarPedidoAsync(CreatePedidoRequest request, CancellationToken ct = default)
    {
        if (request.ClienteId == Guid.Empty)
            throw new FriendlyException("Cliente inválido.");

        if (request.RestauranteId == Guid.Empty)
            throw new FriendlyException("Restaurante inválido.");

        if (request.Itens is null || request.Itens.Count == 0)
            throw new FriendlyException("Adicione ao menos um item no carrinho.");

        var restaurante = await _db.Restaurantes.FirstOrDefaultAsync(r => r.Id == request.RestauranteId && r.Ativo, ct);
        if (restaurante is null)
            throw new FriendlyException("Restaurante não encontrado.");

        var cliente = await _db.Clientes.FirstOrDefaultAsync(c => c.Id == request.ClienteId && c.Ativo, ct);
        if (cliente is null)
            throw new FriendlyException("Cliente não encontrado.");

        var produtoIds = request.Itens.Select(i => i.ProdutoId).Distinct().ToList();
        var produtos = await _db.Produtos.Where(p => produtoIds.Contains(p.Id) && p.RestauranteId == request.RestauranteId && p.Ativo).ToListAsync(ct);

        if (produtos.Count != produtoIds.Count)
            throw new FriendlyException("Um ou mais produtos do carrinho não estão disponíveis.");

        var pedido = new Pedido
        {
            ClienteId = request.ClienteId,
            RestauranteId = request.RestauranteId,
            Observacoes = request.Observacoes?.Trim()
        };

        foreach (var item in request.Itens)
        {
            if (item.Quantidade <= 0)
                throw new FriendlyException("Quantidade inválida no carrinho.");

            var produto = produtos.First(p => p.Id == item.ProdutoId);
            pedido.Itens.Add(new ItemPedido
            {
                PedidoId = pedido.Id,
                ProdutoId = produto.Id,
                Quantidade = item.Quantidade,
                PrecoUnitario = produto.Preco
            });
        }

        await _pedidos.AddAsync(pedido, ct);

        var loaded = await _pedidos.GetByIdAsync(pedido.Id, ct);
        return loaded?.ToDto() ?? throw new FriendlyException("Não foi possível criar o pedido.");
    }

    public async Task<List<PedidoDto>> ListPedidosAsync(Guid clienteId, CancellationToken ct = default)
    {
        var list = await _pedidos.ListByClienteAsync(clienteId, ct);
        return list.Select(p => p.ToDto()).ToList();
    }

    public async Task FavoritarAsync(Guid clienteId, Guid restauranteId, CancellationToken ct = default)
    {
        var exists = await _db.FavoritosRestaurantes.AnyAsync(f => f.ClienteId == clienteId && f.RestauranteId == restauranteId, ct);
        if (exists) return;

        _db.FavoritosRestaurantes.Add(new FavoritoRestaurante
        {
            ClienteId = clienteId,
            RestauranteId = restauranteId
        });

        await _db.SaveChangesAsync(ct);
    }

    public async Task DesfavoritarAsync(Guid clienteId, Guid restauranteId, CancellationToken ct = default)
    {
        var fav = await _db.FavoritosRestaurantes.FirstOrDefaultAsync(f => f.ClienteId == clienteId && f.RestauranteId == restauranteId, ct);
        if (fav is null) return;
        _db.FavoritosRestaurantes.Remove(fav);
        await _db.SaveChangesAsync(ct);
    }
}

