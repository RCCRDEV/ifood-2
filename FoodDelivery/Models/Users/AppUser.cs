using System.ComponentModel.DataAnnotations;

namespace FoodDelivery.Models.Users;

public abstract class AppUser : FoodDelivery.Models.BaseEntity
{
    [Required]
    [MaxLength(120)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [MaxLength(140)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public byte[] PasswordHash { get; set; } = Array.Empty<byte>();

    [Required]
    public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();

    public bool Ativo { get; set; } = true;
}

