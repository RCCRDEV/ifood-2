using System.ComponentModel.DataAnnotations;

namespace FoodDelivery.Models;

public sealed class ItemPedido
{
    public Guid PedidoId { get; set; }
    public Pedido? Pedido { get; set; }

    public Guid ProdutoId { get; set; }
    public Produto? Produto { get; set; }

    [Range(1, 999)]
    public int Quantidade { get; set; }

    [Range(0, 999999)]
    public decimal PrecoUnitario { get; set; }

    public decimal Subtotal => Quantidade * PrecoUnitario;
}

