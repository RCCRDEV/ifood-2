using FoodDelivery.Data;
using FoodDelivery.DTOs;
using FoodDelivery.DTOs.Requests;
using FoodDelivery.Helpers;
using FoodDelivery.Models;
using FoodDelivery.Models.Enums;
using FoodDelivery.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Services;

public sealed class RestauranteService : IRestauranteService
{
    private readonly FoodDeliveryDbContext _db;
    private readonly IProdutoRepository _produtos;
    private readonly IPedidoRepository _pedidos;

    public RestauranteService(
        FoodDeliveryDbContext db,
        IProdutoRepository produtos,
        IPedidoRepository pedidos)
    {
        _db = db;
        _produtos = produtos;
        _pedidos = pedidos;
    }

    public async Task<List<ProdutoDto>> ListProdutosAsync(Guid restauranteId, CancellationToken ct = default)
    {
        var list = await _produtos.ListByRestauranteAsync(restauranteId, ct);
        return list.Select(p => p.ToDto()).ToList();
    }

    public async Task<ProdutoDto> UpsertProdutoAsync(UpsertProdutoRequest request, CancellationToken ct = default)
    {
        if (request.RestauranteId == Guid.Empty)
            throw new FriendlyException("Restaurante inválido.");

        if (string.IsNullOrWhiteSpace(request.Nome))
            throw new FriendlyException("Informe o nome do produto.");

        if (request.Preco <= 0)
            throw new FriendlyException("Informe um preço válido.");

        var restauranteExists = await _db.Restaurantes.AnyAsync(r => r.Id == request.RestauranteId && r.Ativo, ct);
        if (!restauranteExists)
            throw new FriendlyException("Restaurante não encontrado.");

        Produto produto;

        if (request.Id is Guid id && id != Guid.Empty)
        {
            produto = await _db.Produtos.FirstOrDefaultAsync(p => p.Id == id && p.RestauranteId == request.RestauranteId, ct)
                ?? throw new FriendlyException("Produto não encontrado.");

            if ((produto is Prato && request.Tipo != TipoProduto.Prato) || (produto is Bebida && request.Tipo != TipoProduto.Bebida))
                throw new FriendlyException("Não é possível alterar o tipo do produto.");
        }
        else
        {
            produto = request.Tipo switch
            {
                TipoProduto.Prato => new Prato(),
                TipoProduto.Bebida => new Bebida(),
                _ => throw new FriendlyException("Tipo de produto inválido.")
            };

            produto.RestauranteId = request.RestauranteId;
        }

        produto.Nome = request.Nome.Trim();
        produto.Descricao = request.Descricao?.Trim();
        produto.Preco = request.Preco;
        produto.Ativo = request.Ativo;

        if (produto is Prato prato)
        {
            prato.TempoPreparoMin = request.TempoPreparoMin ?? prato.TempoPreparoMin;
            prato.ObservacoesPreparo = request.ObservacoesPreparo?.Trim();
        }

        if (produto is Bebida bebida)
        {
            bebida.VolumeMl = request.VolumeMl ?? bebida.VolumeMl;
            bebida.Alcoolica = request.Alcoolica ?? bebida.Alcoolica;
        }

        var isUpdate = request.Id is Guid existingId && existingId != Guid.Empty;
        if (isUpdate)
            await _produtos.UpdateAsync(produto, ct);
        else
            await _produtos.AddAsync(produto, ct);

        var saved = await _produtos.GetByIdAsync(produto.Id, ct);
        return saved?.ToDto() ?? produto.ToDto();
    }

    public async Task ExcluirProdutoAsync(Guid produtoId, CancellationToken ct = default)
    {
        var produto = await _produtos.GetByIdAsync(produtoId, ct);
        if (produto is null)
            return;
        await _produtos.DeleteAsync(produto, ct);
    }

    public async Task<List<PedidoDto>> ListPedidosAsync(Guid restauranteId, CancellationToken ct = default)
    {
        var list = await _pedidos.ListByRestauranteAsync(restauranteId, ct);
        return list.Select(p => p.ToDto()).ToList();
    }

    public async Task AtualizarStatusAsync(Guid pedidoId, PedidoStatus status, CancellationToken ct = default)
    {
        await _pedidos.UpdateStatusAsync(pedidoId, status, ct);
    }
}
