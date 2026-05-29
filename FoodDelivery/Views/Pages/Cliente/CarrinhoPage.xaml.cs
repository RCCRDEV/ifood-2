using FoodDelivery.Helpers;
using FoodDelivery.Services;
using FoodDelivery.Views.Windows;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;

namespace FoodDelivery.Views.Pages.Cliente;

public partial class CarrinhoPage : Page
{
    private readonly AppSession _session;
    private readonly IServiceProvider _services;

    public CarrinhoPage(AppSession session, IServiceProvider services)
    {
        _session = session;
        _services = services;
        InitializeComponent();
        Loaded += CarrinhoPage_Loaded;
    }

    private void CarrinhoPage_Loaded(object sender, RoutedEventArgs e)
    {
        Reload();
    }

    private void Reload()
    {
        var items = _session.Cart.Items.ToList();
        CartItems.ItemsSource = items;
        EmptyCartPanel.Visibility = items.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        TotalText.Text = $"{_session.Cart.Total:C}";
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

    private void Checkout_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (_session.Cart.RestauranteId is null)
                throw new FriendlyException("Carrinho vazio.");

            var window = _services.GetRequiredService<CheckoutWindow>();
            window.Owner = Window.GetWindow(this);
            window.ShowDialog();
            Reload();
        }
        catch (FriendlyException ex)
        {
            MessageBox.Show(ex.Message, "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception)
        {
            MessageBox.Show("Não foi possível abrir o checkout.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
