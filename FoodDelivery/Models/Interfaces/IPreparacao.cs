namespace FoodDelivery.Models.Interfaces;

public interface IPreparacao
{
    int TempoPreparoMin { get; set; }
    string? ObservacoesPreparo { get; set; }
}

