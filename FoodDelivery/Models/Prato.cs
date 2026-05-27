using FoodDelivery.Models.Enums;
using FoodDelivery.Models.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace FoodDelivery.Models;

public sealed class Prato : Produto, IPreparacao
{
    public override TipoProduto Tipo => TipoProduto.Prato;

    [Range(0, 999)]
    public int TempoPreparoMin { get; set; } = 15;

    [MaxLength(300)]
    public string? ObservacoesPreparo { get; set; }
}

