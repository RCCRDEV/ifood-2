using FoodDelivery.Services;
using System.Windows;
using System.Windows.Controls;

namespace FoodDelivery.Views.Pages.Admin;

public partial class PedidosAdminPage : Page
{
    private readonly IAdminService _service;

    public PedidosAdminPage(IAdminService service)
    {
        _service = service;
        InitializeComponent();
        Loaded += PedidosAdminPage_Loaded;
    }

    private async void PedidosAdminPage_Loaded(object sender, RoutedEventArgs e)
    {
        await ReloadAsync();
    }

    private async Task ReloadAsync()
    {
        Grid.ItemsSource = await _service.ListPedidosAsync();
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

