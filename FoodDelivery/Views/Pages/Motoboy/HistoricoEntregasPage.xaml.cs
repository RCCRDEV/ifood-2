using FoodDelivery.DTOs;
using FoodDelivery.Helpers;
using FoodDelivery.Models.Enums;
using FoodDelivery.Services;
using System.Windows;
using System.Windows.Controls;

namespace FoodDelivery.Views.Pages.Motoboy;

public partial class HistoricoEntregasPage : Page
{
    private readonly IMotoboyService _service;
    private readonly AppSession _session;

    public HistoricoEntregasPage(IMotoboyService service, AppSession session)
    {
        _service = service;
        _session = session;
        InitializeComponent();
        Loaded += HistoricoEntregasPage_Loaded;
    }

    private async void HistoricoEntregasPage_Loaded(object sender, RoutedEventArgs e)
    {
        await ReloadAsync();
    }

    private async Task ReloadAsync()
    {
        if (_session.CurrentUser is not global::FoodDelivery.Models.Users.Motoboy motoboy) return;
        Grid.ItemsSource = await _service.HistoricoAsync(motoboy.Id);
    }

    private async void Delivered_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (Grid.SelectedItem is not PedidoDto pedido)
                throw new FriendlyException("Selecione uma entrega.");

            await _service.AtualizarStatusEntregaAsync(pedido.Id, PedidoStatus.Entregue);
            await ReloadAsync();
            MessageBox.Show("Status atualizado para Entregue.", "Entregas", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (FriendlyException ex)
        {
            MessageBox.Show(ex.Message, "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception)
        {
            MessageBox.Show("Não foi possível atualizar.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
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
