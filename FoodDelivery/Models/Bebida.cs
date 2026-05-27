using FoodDelivery.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace FoodDelivery.Models;

public sealed class Bebida : Produto
{
    public override TipoProduto Tipo => TipoProduto.Bebida;

    [Range(0, 5000)]
    public int VolumeMl { get; set; } = 350;

    public bool Alcoolica { get; set; }
}

