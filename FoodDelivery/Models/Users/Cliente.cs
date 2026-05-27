using System.ComponentModel.DataAnnotations;

namespace FoodDelivery.Models.Users;

public sealed class Cliente : AppUser
{
    [MaxLength(30)]
    public string? Telefone { get; set; }

    [MaxLength(220)]
    public string? Endereco { get; set; }

    public ICollection<FoodDelivery.Models.FavoritoRestaurante> Favoritos { get; set; } = new List<FoodDelivery.Models.FavoritoRestaurante>();
    public ICollection<FoodDelivery.Models.Pedido> Pedidos { get; set; } = new List<FoodDelivery.Models.Pedido>();
}

