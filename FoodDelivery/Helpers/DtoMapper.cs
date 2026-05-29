using FoodDelivery.DTOs;
using FoodDelivery.Models;
using FoodDelivery.Models.Enums;
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

        var statusLabel = p.Status switch
        {
            PedidoStatus.AguardandoConfirmacaoLoja => "Aguardando confirmação da loja",
            PedidoStatus.Recebido => "Recebido",
            PedidoStatus.EmPreparo => "Em preparo",
            PedidoStatus.SaiuParaEntrega => "Disponível para entrega",
            PedidoStatus.EmEntrega => "Em entrega",
            PedidoStatus.Entregue => "Entregue",
            PedidoStatus.Cancelado => "Cancelado",
            _ => p.Status.ToString()
        };

        var metodoLabel = p.MetodoPagamento switch
        {
            MetodoPagamento.Pix => "PIX",
            MetodoPagamento.CartaoCredito => "Cartão de crédito",
            MetodoPagamento.CartaoDebito => "Cartão de débito",
            MetodoPagamento.Dinheiro => "Dinheiro",
            _ => p.MetodoPagamento.ToString()
        };

        var statusPagLabel = p.StatusPagamento switch
        {
            StatusPagamento.Pendente => p.MetodoPagamento == MetodoPagamento.Dinheiro ? "Pagamento na entrega" : "Pendente",
            StatusPagamento.Aprovado => "Aprovado",
            StatusPagamento.Recusado => "Recusado",
            StatusPagamento.Estornado => "Estornado",
            _ => p.StatusPagamento.ToString()
        };

        var pagamentoLabel = $"{metodoLabel} • {statusPagLabel}";
        var hasMotivo = !string.IsNullOrWhiteSpace(p.CancelamentoMotivo);
        var canCancel = p.Status == PedidoStatus.AguardandoConfirmacaoLoja;

        return new PedidoDto(
            p.Id,
            p.Id.ToString("N")[..8].ToUpperInvariant(),
            p.DataPedidoUtc,
            p.Restaurante?.Nome ?? "(Restaurante)",
            p.Cliente?.Nome ?? "(Cliente)",
            p.Cliente?.Telefone,
            p.Cliente?.Endereco,
            p.Motoboy?.Nome,
            p.Status,
            statusLabel,
            p.MetodoPagamento,
            p.StatusPagamento,
            pagamentoLabel,
            p.Observacoes,
            p.CancelamentoMotivo,
            hasMotivo,
            canCancel,
            p.Total,
            itens
        );
    }

    public static UserDto ToDto(this AppUser u)
        => new(u.Id, u.Nome, u.Email, u.GetType().Name, u.Ativo);
}
