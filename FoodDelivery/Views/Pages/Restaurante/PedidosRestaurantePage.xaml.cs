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

    private async void Confirmar_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (PedidosGrid.SelectedItem is not PedidoDto pedido)
                throw new FriendlyException("Selecione um pedido.");

            await _service.ConfirmarPedidoAsync(pedido.Id);
            await ReloadAsync();
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

    private async void Recusar_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (PedidosGrid.SelectedItem is not PedidoDto pedido)
                throw new FriendlyException("Selecione um pedido.");

            var motivo = PromptText("Recusar pedido", "Informe o motivo da recusa:", "Ex.: item esgotado");
            if (motivo is null)
                return;
            await _service.RecusarPedidoAsync(pedido.Id, motivo);
            await ReloadAsync();
        }
        catch (FriendlyException ex)
        {
            MessageBox.Show(ex.Message, "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception)
        {
            MessageBox.Show("Não foi possível recusar o pedido.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
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

    private static string? PromptText(string title, string label, string placeholder)
    {
        var window = new Window
        {
            Title = title,
            Width = 520,
            Height = 220,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            ResizeMode = ResizeMode.NoResize,
            Background = (System.Windows.Media.Brush)Application.Current.Resources["BrushBg"],
            Foreground = (System.Windows.Media.Brush)Application.Current.Resources["BrushText"]
        };

        var root = new Grid { Margin = new Thickness(18) };
        root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        var textLabel = new TextBlock
        {
            Text = label,
            FontWeight = FontWeights.SemiBold
        };
        root.Children.Add(textLabel);

        var box = new TextBox
        {
            Margin = new Thickness(0, 10, 0, 0),
            Text = placeholder
        };
        Grid.SetRow(box, 1);
        root.Children.Add(box);

        var buttons = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(0, 16, 0, 0) };
        var cancel = new Button { Content = "Voltar", Width = 120 };
        cancel.SetResourceReference(FrameworkElement.StyleProperty, "SecondaryButton");
        cancel.Click += (_, _) => window.DialogResult = false;

        var ok = new Button { Content = "Recusar", Width = 120, Margin = new Thickness(10, 0, 0, 0) };
        ok.SetResourceReference(FrameworkElement.StyleProperty, "DangerButton");
        ok.Click += (_, _) => window.DialogResult = true;

        buttons.Children.Add(cancel);
        buttons.Children.Add(ok);
        Grid.SetRow(buttons, 2);
        root.Children.Add(buttons);

        window.Content = root;
        window.Loaded += (_, _) =>
        {
            box.Focus();
            box.SelectAll();
        };

        var result = window.ShowDialog();
        if (result != true) return null;
        var value = box.Text?.Trim();
        return string.IsNullOrWhiteSpace(value) ? "Recusado pela loja." : value;
    }
}
