using FoodDelivery.DTOs.Requests;
using FoodDelivery.Helpers;
using FoodDelivery.Models.Enums;
using FoodDelivery.Models.Users;
using FoodDelivery.Services;
using System.Windows;

namespace FoodDelivery.Views.Windows;

public partial class CheckoutWindow : Window
{
    private readonly IClienteService _service;
    private readonly AppSession _session;

    public CheckoutWindow(IClienteService service, AppSession session)
    {
        _service = service;
        _session = session;
        InitializeComponent();
        Loaded += CheckoutWindow_Loaded;
    }

    private void CheckoutWindow_Loaded(object sender, RoutedEventArgs e)
    {
        var items = _session.Cart.Items.ToList();
        ResumoItems.ItemsSource = items;
        TotalText.Text = $"{_session.Cart.Total:C}";
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private async void Confirm_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (_session.CurrentUser is not Cliente cliente)
                throw new FriendlyException("Sessão inválida.");

            if (_session.Cart.RestauranteId is null || _session.Cart.Items.Count == 0)
                throw new FriendlyException("Carrinho vazio.");

            var metodo = GetMetodoPagamento();

            var request = new CreatePedidoRequest(
                cliente.Id,
                _session.Cart.RestauranteId.Value,
                metodo,
                ObsTextBox.Text,
                _session.Cart.Items.Select(i => new CreatePedidoItemRequest(i.ProdutoId, i.Quantidade)).ToList()
            );

            var pedido = await _service.CriarPedidoAsync(request);
            _session.Cart.Clear();
            MessageBox.Show($"Pedido enviado para a loja confirmar.\n\nPagamento: {pedido.PagamentoLabel}\nTotal: {pedido.Total:C}", "Pedido confirmado", MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        }
        catch (FriendlyException ex)
        {
            MessageBox.Show(ex.Message, "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception)
        {
            MessageBox.Show("Não foi possível confirmar o pedido.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private MetodoPagamento GetMetodoPagamento()
    {
        if (PixRadio.IsChecked == true) return MetodoPagamento.Pix;
        if (CreditoRadio.IsChecked == true) return MetodoPagamento.CartaoCredito;
        if (DebitoRadio.IsChecked == true) return MetodoPagamento.CartaoDebito;
        return MetodoPagamento.Dinheiro;
    }
}

