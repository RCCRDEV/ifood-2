using FoodDelivery.DTOs;
using FoodDelivery.Helpers;
using FoodDelivery.Services;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace FoodDelivery.Views.Pages.Motoboy;

public partial class EntregasDisponiveisPage : Page
{
    private readonly IMotoboyService _service;
    private readonly AppSession _session;

    public EntregasDisponiveisPage(IMotoboyService service, AppSession session)
    {
        _service = service;
        _session = session;
        InitializeComponent();
        Loaded += EntregasDisponiveisPage_Loaded;
    }

    private async void EntregasDisponiveisPage_Loaded(object sender, RoutedEventArgs e)
    {
        await ReloadAsync();
    }

    private async Task ReloadAsync()
    {
        Grid.ItemsSource = await _service.ListEntregasDisponiveisAsync();
    }

    private async void Accept_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (_session.CurrentUser is not global::FoodDelivery.Models.Users.Motoboy motoboy)
                throw new FriendlyException("Sessão inválida.");

            if (Grid.SelectedItem is not PedidoDto pedido)
                throw new FriendlyException("Selecione uma entrega.");

            await _service.AceitarEntregaAsync(pedido.Id, motoboy.Id);
            await ReloadAsync();
            MessageBox.Show("Entrega aceita com sucesso.", "Entregas", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (FriendlyException ex)
        {
            MessageBox.Show(ex.Message, "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception)
        {
            MessageBox.Show("Não foi possível aceitar.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void CopyAddress_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (Grid.SelectedItem is not PedidoDto pedido)
                throw new FriendlyException("Selecione uma entrega.");

            if (string.IsNullOrWhiteSpace(pedido.ClienteEndereco))
                throw new FriendlyException("Cliente sem endereço cadastrado.");

            Clipboard.SetText(pedido.ClienteEndereco.Trim());
            MessageBox.Show("Endereço copiado.", "Entregas", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (FriendlyException ex)
        {
            MessageBox.Show(ex.Message, "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception)
        {
            MessageBox.Show("Não foi possível copiar.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void OpenMaps_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (Grid.SelectedItem is not PedidoDto pedido)
                throw new FriendlyException("Selecione uma entrega.");

            if (string.IsNullOrWhiteSpace(pedido.ClienteEndereco))
                throw new FriendlyException("Cliente sem endereço cadastrado.");

            var url = "https://www.google.com/maps/search/?api=1&query=" + Uri.EscapeDataString(pedido.ClienteEndereco.Trim());
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
        catch (FriendlyException ex)
        {
            MessageBox.Show(ex.Message, "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception)
        {
            MessageBox.Show("Não foi possível abrir o Maps.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
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
