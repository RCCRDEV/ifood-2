using FoodDelivery.DTOs;
using FoodDelivery.Helpers;
using FoodDelivery.Models.Enums;
using FoodDelivery.Models.Users;
using FoodDelivery.Services;
using System.Windows;
using System.Windows.Controls;

namespace FoodDelivery.Views.Pages.Restaurante;

public partial class PedidosRestaurantePage : Page
{
    private readonly IRestauranteService _service;
    private readonly AppSession _session;

    public PedidosRestaurantePage(IRestauranteService service, AppSession session)
    {
        _service = service;
        _session = session;
        InitializeComponent();
        Loaded += PedidosRestaurantePage_Loaded;
    }

    private async void PedidosRestaurantePage_Loaded(object sender, RoutedEventArgs e)
    {
        await ReloadAsync();
    }

    private Guid GetRestauranteId()
    {
        if (_session.CurrentUser is not RestauranteUser user)
            throw new FriendlyException("Sessão inválida.");
        return user.RestauranteId;
    }

    private async Task ReloadAsync()
    {
        var list = await _service.ListPedidosAsync(GetRestauranteId());
        PedidosGrid.ItemsSource = list;
    }

    private async Task UpdateStatusAsync(PedidoStatus status)
    {
        if (PedidosGrid.SelectedItem is not PedidoDto pedido)
            throw new FriendlyException("Selecione um pedido.");

        await _service.AtualizarStatusAsync(pedido.Id, status);
        await ReloadAsync();
    }

    private async void EmPreparo_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await UpdateStatusAsync(PedidoStatus.EmPreparo);
        }
        catch (FriendlyException ex)
        {
            MessageBox.Show(ex.Message, "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception)
        {
            MessageBox.Show("Não foi possível atualizar o pedido.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void Saiu_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await UpdateStatusAsync(PedidoStatus.SaiuParaEntrega);
        }
        catch (FriendlyException ex)
        {
            MessageBox.Show(ex.Message, "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception)
        {
            MessageBox.Show("Não foi possível atualizar o pedido.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void Entregue_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await UpdateStatusAsync(PedidoStatus.Entregue);
        }
        catch (FriendlyException ex)
        {
            MessageBox.Show(ex.Message, "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception)
        {
            MessageBox.Show("Não foi possível atualizar o pedido.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
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

