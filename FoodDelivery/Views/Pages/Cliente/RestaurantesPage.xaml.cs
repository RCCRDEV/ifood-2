using FoodDelivery.DTOs;
using FoodDelivery.Helpers;
using FoodDelivery.Services;
using System.Windows;
using System.Windows.Controls;

namespace FoodDelivery.Views.Pages.Cliente;

public partial class RestaurantesPage : Page
{
    private readonly IClienteService _service;
    private readonly AppSession _session;

    private List<RestauranteDto> _restaurantes = [];
    private Guid? _selectedRestauranteId;

    public RestaurantesPage(IClienteService service, AppSession session)
    {
        _service = service;
        _session = session;
        InitializeComponent();
        Loaded += RestaurantesPage_Loaded;
    }

    private async void RestaurantesPage_Loaded(object sender, RoutedEventArgs e)
    {
        await ReloadAsync();
    }

    private async Task ReloadAsync()
    {
        _restaurantes = await _service.ListRestaurantesAsync();
        RestaurantesGrid.ItemsSource = _restaurantes;
        SelectedRestauranteText.Text = "Selecione um restaurante para ver o cardápio.";
        ProdutosGrid.ItemsSource = null;
    }

    private async void RestaurantesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (RestaurantesGrid.SelectedItem is not RestauranteDto restaurante) return;
        _selectedRestauranteId = restaurante.Id;
        SelectedRestauranteText.Text = $"{restaurante.Nome} • {restaurante.Endereco}";
        var produtos = await _service.ListCardapioAsync(restaurante.Id);
        ProdutosGrid.ItemsSource = produtos;
    }

    private void AddToCart_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedRestauranteId is null) return;
        if (sender is not Button btn || btn.Tag is not ProdutoDto produto) return;

        _session.Cart.Add(produto.RestauranteId, produto.Id, produto.Nome, produto.Preco, 1);
        MessageBox.Show("Produto adicionado ao carrinho.", "Carrinho", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}

