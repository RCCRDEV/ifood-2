using FoodDelivery.Models.Users;

namespace FoodDelivery.Models;

public sealed class FavoritoRestaurante
{
    public Guid ClienteId { get; set; }
    public Cliente? Cliente { get; set; }

    public Guid RestauranteId { get; set; }
    public Restaurante? Restaurante { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}

