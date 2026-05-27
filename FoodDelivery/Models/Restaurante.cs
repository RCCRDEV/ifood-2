using System.ComponentModel.DataAnnotations;

namespace FoodDelivery.Models;

public sealed class Restaurante : BaseEntity
{
    [Required]
    [MaxLength(120)]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(400)]
    public string? Descricao { get; set; }

    [MaxLength(220)]
    public string? Endereco { get; set; }

    [MaxLength(30)]
    public string? Telefone { get; set; }

    public bool Ativo { get; set; } = true;

    public ICollection<Produto> Produtos { get; set; } = new List<Produto>();
    public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
    public ICollection<FavoritoRestaurante> Favoritos { get; set; } = new List<FavoritoRestaurante>();
}

