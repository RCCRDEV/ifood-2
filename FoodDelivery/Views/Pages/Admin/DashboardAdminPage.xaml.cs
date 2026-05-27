using FoodDelivery.Services;
using System.Windows;
using System.Windows.Controls;

namespace FoodDelivery.Views.Pages.Admin;

public partial class DashboardAdminPage : Page
{
    private readonly IAdminService _service;

    public DashboardAdminPage(IAdminService service)
    {
        _service = service;
        InitializeComponent();
        Loaded += DashboardAdminPage_Loaded;
    }

    private async void DashboardAdminPage_Loaded(object sender, RoutedEventArgs e)
    {
        await ReloadAsync();
    }

    private async Task ReloadAsync()
    {
        var dto = await _service.GetDashboardAsync();
        UsuariosText.Text = dto.TotalUsuarios.ToString();
        RestaurantesText.Text = dto.TotalRestaurantes.ToString();
        PedidosText.Text = dto.TotalPedidos.ToString();
        PedidosHojeText.Text = dto.PedidosHoje.ToString();
    }

    private async void Refresh_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await ReloadAsync();
        }
        catch (Exception)
        {
            MessageBox.Show("Não foi possível atualizar.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}

