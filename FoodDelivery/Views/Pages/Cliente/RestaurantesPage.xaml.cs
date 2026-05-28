using FoodDelivery.DTOs;
using FoodDelivery.Helpers;
using FoodDelivery.Models.Enums;
using FoodDelivery.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace FoodDelivery.Views.Pages.Cliente;

public partial class RestaurantesPage : Page
{
    private readonly IClienteService _service;
    private readonly AppSession _session;

    private List<RestauranteDto> _restaurantes = [];
    private List<RestauranteDto> _restaurantesFiltrados = [];
    private RestauranteDto? _restauranteSelecionado;
    private List<ProdutoDto> _produtos = [];
    private TipoProduto? _categoriaSelecionada;

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
        _restaurantesFiltrados = _restaurantes;
        RestaurantesListBox.ItemsSource = _restaurantesFiltrados;
        RestaurantesListBox.SelectedIndex = -1;

        _restauranteSelecionado = null;
        _produtos = [];
        _categoriaSelecionada = null;

        RestauranteTitleText.Text = "Escolha um restaurante";
        RestauranteSubtitleText.Text = "Selecione ao lado para ver o cardápio e adicionar itens ao carrinho.";
        ProdutosItems.ItemsSource = null;
        BuildCategorias();
    }

    private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        var query = (SearchTextBox.Text ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(query))
        {
            _restaurantesFiltrados = _restaurantes;
        }
        else
        {
            _restaurantesFiltrados = _restaurantes
                .Where(r =>
                    r.Nome.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    (r.Endereco ?? string.Empty).Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    (r.Descricao ?? string.Empty).Contains(query, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        RestaurantesListBox.ItemsSource = _restaurantesFiltrados;
    }

    private async void RestaurantesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (RestaurantesListBox.SelectedItem is not RestauranteDto restaurante) return;

        _restauranteSelecionado = restaurante;
        RestauranteTitleText.Text = restaurante.Nome;
        RestauranteSubtitleText.Text = string.IsNullOrWhiteSpace(restaurante.Endereco) ? "Entrega rápida" : restaurante.Endereco!;

        _categoriaSelecionada = null;
        SetCategoriaVisual();

        _produtos = await _service.ListCardapioAsync(restaurante.Id);
        RenderProdutos();
    }

    private void AddToCart_Click(object sender, RoutedEventArgs e)
    {
        if (_restauranteSelecionado is null) return;
        if (sender is not Button btn || btn.Tag is not ProdutoDto produto) return;

        _session.Cart.Add(produto.RestauranteId, produto.Id, produto.Nome, produto.Preco, 1);
        MessageBox.Show("Produto adicionado ao carrinho.", "Carrinho", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void BuildCategorias()
    {
        CategoriaPanel.Children.Clear();

        CategoriaPanel.Children.Add(BuildChip("Todos", null));
        CategoriaPanel.Children.Add(BuildChip("Pratos", TipoProduto.Prato));
        CategoriaPanel.Children.Add(BuildChip("Bebidas", TipoProduto.Bebida));

        SetCategoriaVisual();
    }

    private ToggleButton BuildChip(string label, TipoProduto? tipo)
    {
        var chip = new ToggleButton
        {
            Content = label,
            Tag = tipo,
            Margin = new Thickness(0, 0, 10, 10),
            MinWidth = 90,
            IsChecked = tipo == _categoriaSelecionada
        };

        chip.SetResourceReference(StyleProperty, "ChipToggle");
        chip.Checked += Categoria_Checked;
        chip.Unchecked += Categoria_Unchecked;
        return chip;
    }

    private void Categoria_Checked(object sender, RoutedEventArgs e)
    {
        if (sender is not ToggleButton btn) return;

        _categoriaSelecionada = (TipoProduto?)btn.Tag;
        SetCategoriaVisual(except: btn);
        RenderProdutos();
    }

    private void Categoria_Unchecked(object sender, RoutedEventArgs e)
    {
        if (sender is not ToggleButton btn) return;
        if (btn.IsKeyboardFocusWithin) return;
    }

    private void SetCategoriaVisual(ToggleButton? except = null)
    {
        foreach (var child in CategoriaPanel.Children)
        {
            if (child is not ToggleButton btn) continue;
            if (except is not null && ReferenceEquals(btn, except)) continue;
            btn.IsChecked = Equals(btn.Tag, _categoriaSelecionada);
        }
    }

    private void RenderProdutos()
    {
        if (_restauranteSelecionado is null)
        {
            ProdutosItems.ItemsSource = null;
            return;
        }

        IEnumerable<ProdutoDto> items = _produtos;
        if (_categoriaSelecionada is TipoProduto tipo)
            items = items.Where(p => p.Tipo == tipo);

        ProdutosItems.ItemsSource = items.ToList();
    }
}
