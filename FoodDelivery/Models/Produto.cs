using FoodDelivery.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace FoodDelivery.Models;

public abstract class Produto : BaseEntity
{
    public Guid RestauranteId { get; set; }
    public Restaurante? Restaurante { get; set; }

    [Required]
    [MaxLength(140)]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Descricao { get; set; }

    [Range(0, 999999)]
    public decimal Preco { get; set; }

    public bool Ativo { get; set; } = true;

    public abstract TipoProduto Tipo { get; }

    public ICollection<ItemPedido> ItensPedido { get; set; } = new List<ItemPedido>();
}

