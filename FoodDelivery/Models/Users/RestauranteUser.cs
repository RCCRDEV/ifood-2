using System.ComponentModel.DataAnnotations;

namespace FoodDelivery.Models.Users;

public sealed class RestauranteUser : AppUser
{
    public Guid RestauranteId { get; set; }

    [MaxLength(120)]
    public string? Cargo { get; set; }

    public FoodDelivery.Models.Restaurante? Restaurante { get; set; }
}

