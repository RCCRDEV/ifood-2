using FoodDelivery.DTOs.Requests;
using FoodDelivery.Helpers;
using FoodDelivery.Services;
using System.Windows;
using System.Windows.Controls;

namespace FoodDelivery.Views.Pages.Cliente;

public partial class CarrinhoPage : Page
{
    private readonly AppSession _session;
    private readonly IClienteService _service;

    public CarrinhoPage(AppSession session, IClienteService service)
    {
        _session = session;
        _service = service;
        InitializeComponent();
        Loaded += CarrinhoPage_Loaded;
    }

    private void CarrinhoPage_Loaded(object sender, RoutedEventArgs e)
    {
        Reload();
    }

    private void Reload()
    {
        CartGrid.ItemsSource = null;
        CartGrid.ItemsSource = _session.Cart.Items;
        TotalText.Text = $"Total: {_session.Cart.Total:C}";
    }

    private void Remove_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button btn || btn.Tag is not CartItem item) return;
        _session.Cart.Remove(item.ProdutoId);
        Reload();
    }

    private void Clear_Click(object sender, RoutedEventArgs e)
    {
        _session.Cart.Clear();
        Reload();
    }

    private async void Checkout_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (_session.CurrentUser is not global::FoodDelivery.Models.Users.Cliente cliente)
                throw new FriendlyException("Sessão inválida.");

            if (_session.Cart.RestauranteId is null)
                throw new FriendlyException("Carrinho vazio.");

            var request = new CreatePedidoRequest(
                cliente.Id,
                _session.Cart.RestauranteId.Value,
                ObsTextBox.Text,
                _session.Cart.Items.Select(i => new CreatePedidoItemRequest(i.ProdutoId, i.Quantidade)).ToList()
            );

            var pedido = await _service.CriarPedidoAsync(request);
            _session.Cart.Clear();
            Reload();

            MessageBox.Show($"Pedido criado com sucesso. Total: {pedido.Total:C}", "Pedido", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (FriendlyException ex)
        {
            MessageBox.Show(ex.Message, "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception)
        {
            MessageBox.Show("Não foi possível finalizar o pedido.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
