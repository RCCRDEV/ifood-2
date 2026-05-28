using FoodDelivery.Helpers;
using FoodDelivery.Services;
using System.Windows;
using System.Windows.Controls;

namespace FoodDelivery.Views.Pages.Cliente;

public partial class PedidosClientePage : Page
{
    private readonly IClienteService _service;
    private readonly AppSession _session;

    public PedidosClientePage(IClienteService service, AppSession session)
    {
        _service = service;
        _session = session;
        InitializeComponent();
        Loaded += PedidosClientePage_Loaded;
    }

    private async void PedidosClientePage_Loaded(object sender, RoutedEventArgs e)
    {
        await ReloadAsync();
    }

    private async Task ReloadAsync()
    {
        if (_session.CurrentUser is not global::FoodDelivery.Models.Users.Cliente cliente) return;
        var list = await _service.ListPedidosAsync(cliente.Id);
        PedidosItems.ItemsSource = list;
        EmptyOrdersPanel.Visibility = list.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
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
