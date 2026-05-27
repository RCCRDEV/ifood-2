using FoodDelivery.DTOs;
using FoodDelivery.Models;
using FoodDelivery.Models.Users;

namespace FoodDelivery.Helpers;

public static class DtoMapper
{
    public static RestauranteDto ToDto(this Restaurante r)
        => new(r.Id, r.Nome, r.Descricao, r.Endereco, r.Telefone, r.Ativo);

    public static ProdutoDto ToDto(this Produto p)
    {
        return p switch
        {
            Prato prato => new ProdutoDto(prato.Id, prato.RestauranteId, prato.Nome, prato.Descricao, prato.Preco, prato.Ativo, prato.Tipo, prato.TempoPreparoMin, null, null),
            Bebida bebida => new ProdutoDto(bebida.Id, bebida.RestauranteId, bebida.Nome, bebida.Descricao, bebida.Preco, bebida.Ativo, bebida.Tipo, null, bebida.VolumeMl, bebida.Alcoolica),
            _ => new ProdutoDto(p.Id, p.RestauranteId, p.Nome, p.Descricao, p.Preco, p.Ativo, p.Tipo, null, null, null)
        };
    }

    public static PedidoDto ToDto(this Pedido p)
    {
        var itens = p.Itens.Select(i => new PedidoItemDto(
            i.ProdutoId,
            i.Produto?.Nome ?? "(Produto)",
            i.Quantidade,
            i.PrecoUnitario,
            i.Subtotal
        )).ToList();

        return new PedidoDto(
            p.Id,
            p.DataPedidoUtc,
            p.Restaurante?.Nome ?? "(Restaurante)",
            p.Cliente?.Nome ?? "(Cliente)",
            p.Motoboy?.Nome,
            p.Status,
            p.Total,
            itens
        );
    }

    public static UserDto ToDto(this AppUser u)
        => new(u.Id, u.Nome, u.Email, u.GetType().Name, u.Ativo);
}
