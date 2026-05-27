using FoodDelivery.Models.Enums;
using FoodDelivery.Models.Users;
using System.ComponentModel.DataAnnotations;

namespace FoodDelivery.Models;

public sealed class Pedido : BaseEntity
{
    public Guid ClienteId { get; set; }
    public Cliente? Cliente { get; set; }

    public Guid RestauranteId { get; set; }
    public Restaurante? Restaurante { get; set; }

    public Guid? MotoboyId { get; set; }
    public Motoboy? Motoboy { get; set; }

    public PedidoStatus Status { get; set; } = PedidoStatus.Recebido;

    [MaxLength(400)]
    public string? Observacoes { get; set; }

    public DateTime DataPedidoUtc { get; set; } = DateTime.UtcNow;

    public ICollection<ItemPedido> Itens { get; set; } = new List<ItemPedido>();

    public decimal Total => Itens.Sum(i => i.Subtotal);
}

