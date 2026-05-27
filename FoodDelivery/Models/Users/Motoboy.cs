using System.ComponentModel.DataAnnotations;

namespace FoodDelivery.Models.Users;

public sealed class Motoboy : AppUser
{
    [MaxLength(20)]
    public string? PlacaVeiculo { get; set; }

    public ICollection<FoodDelivery.Models.Pedido> Entregas { get; set; } = new List<FoodDelivery.Models.Pedido>();
}

