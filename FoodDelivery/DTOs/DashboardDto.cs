namespace FoodDelivery.DTOs;

public sealed record DashboardDto(
    int TotalUsuarios,
    int TotalRestaurantes,
    int TotalPedidos,
    int PedidosHoje
);

