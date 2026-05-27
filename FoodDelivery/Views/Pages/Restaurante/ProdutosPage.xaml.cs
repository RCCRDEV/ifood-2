using FoodDelivery.DTOs;
using FoodDelivery.DTOs.Requests;
using FoodDelivery.Helpers;
using FoodDelivery.Models.Enums;
using FoodDelivery.Models.Users;
using FoodDelivery.Services;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace FoodDelivery.Views.Pages.Restaurante;

public partial class ProdutosPage : Page
{
    private readonly IRestauranteService _service;
    private readonly AppSession _session;

    private Guid? _editingId;

    public ProdutosPage(IRestauranteService service, AppSession session)
    {
        _service = service;
        _session = session;
        InitializeComponent();
        Loaded += ProdutosPage_Loaded;
        TipoCombo.SelectionChanged += TipoCombo_SelectionChanged;
    }

    private async void ProdutosPage_Loaded(object sender, RoutedEventArgs e)
    {
        TipoCombo.ItemsSource = Enum.GetValues(typeof(TipoProduto));
        AtivoCombo.ItemsSource = new[] { true, false };
        AlcoolicaCombo.ItemsSource = new[] { true, false };

        AtivoCombo.SelectedItem = true;
        TipoCombo.SelectedItem = TipoProduto.Prato;
        await ReloadAsync();
        ApplyTipoVisibility();
    }

    private void TipoCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ApplyTipoVisibility();
    }

    private void ApplyTipoVisibility()
    {
        var tipo = (TipoProduto)(TipoCombo.SelectedItem ?? TipoProduto.Prato);
        PratoFields.Visibility = tipo == TipoProduto.Prato ? Visibility.Visible : Visibility.Collapsed;
        BebidaFields.Visibility = tipo == TipoProduto.Bebida ? Visibility.Visible : Visibility.Collapsed;
    }

    private Guid GetRestauranteId()
    {
        if (_session.CurrentUser is not RestauranteUser user)
            throw new FriendlyException("Sessão inválida.");
        return user.RestauranteId;
    }

    private async Task ReloadAsync()
    {
        var restauranteId = GetRestauranteId();
        var list = await _service.ListProdutosAsync(restauranteId);
        ProdutosGrid.ItemsSource = list;
    }

    private void ProdutosGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ProdutosGrid.SelectedItem is not ProdutoDto p) return;

        _editingId = p.Id;
        TipoCombo.SelectedItem = p.Tipo;
        TipoCombo.IsEnabled = false;
        NomeTextBox.Text = p.Nome;
        DescricaoTextBox.Text = p.Descricao ?? string.Empty;
        PrecoTextBox.Text = p.Preco.ToString("0.00", CultureInfo.CurrentCulture);
        AtivoCombo.SelectedItem = p.Ativo;

        TempoPreparoTextBox.Text = (p.TempoPreparoMin ?? 0).ToString(CultureInfo.CurrentCulture);
        VolumeMlTextBox.Text = (p.VolumeMl ?? 0).ToString(CultureInfo.CurrentCulture);
        AlcoolicaCombo.SelectedItem = p.Alcoolica ?? false;

        ApplyTipoVisibility();
    }

    private void New_Click(object sender, RoutedEventArgs e)
    {
        _editingId = null;
        TipoCombo.IsEnabled = true;
        TipoCombo.SelectedItem = TipoProduto.Prato;
        NomeTextBox.Text = string.Empty;
        DescricaoTextBox.Text = string.Empty;
        PrecoTextBox.Text = string.Empty;
        AtivoCombo.SelectedItem = true;
        TempoPreparoTextBox.Text = "15";
        VolumeMlTextBox.Text = "350";
        AlcoolicaCombo.SelectedItem = false;
        ApplyTipoVisibility();
    }

    private async void Save_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var restauranteId = GetRestauranteId();
            var tipo = (TipoProduto)(TipoCombo.SelectedItem ?? TipoProduto.Prato);

            if (!decimal.TryParse(PrecoTextBox.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out var preco))
                throw new FriendlyException("Preço inválido.");

            int? tempoPreparo = null;
            if (tipo == TipoProduto.Prato)
            {
                if (!int.TryParse(TempoPreparoTextBox.Text, out var t))
                    throw new FriendlyException("Tempo de preparo inválido.");
                tempoPreparo = t;
            }

            int? volume = null;
            bool? alcoolica = null;
            if (tipo == TipoProduto.Bebida)
            {
                if (!int.TryParse(VolumeMlTextBox.Text, out var v))
                    throw new FriendlyException("Volume inválido.");
                volume = v;
                alcoolica = (bool?)AlcoolicaCombo.SelectedItem ?? false;
            }

            var request = new UpsertProdutoRequest(
                _editingId,
                restauranteId,
                tipo,
                NomeTextBox.Text,
                DescricaoTextBox.Text,
                preco,
                (bool?)AtivoCombo.SelectedItem ?? true,
                tempoPreparo,
                null,
                volume,
                alcoolica
            );

            await _service.UpsertProdutoAsync(request);
            await ReloadAsync();
            MessageBox.Show("Produto salvo com sucesso.", "Produtos", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (FriendlyException ex)
        {
            MessageBox.Show(ex.Message, "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception)
        {
            MessageBox.Show("Não foi possível salvar o produto.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void Delete_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (_editingId is null)
                throw new FriendlyException("Selecione um produto para excluir.");

            await _service.ExcluirProdutoAsync(_editingId.Value);
            New_Click(sender, e);
            await ReloadAsync();
            MessageBox.Show("Produto excluído.", "Produtos", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (FriendlyException ex)
        {
            MessageBox.Show(ex.Message, "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception)
        {
            MessageBox.Show("Não foi possível excluir.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
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

