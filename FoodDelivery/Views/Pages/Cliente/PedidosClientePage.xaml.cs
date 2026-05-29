using FoodDelivery.Helpers;
using FoodDelivery.DTOs;
using FoodDelivery.Services;
using System.Windows;
using System.Windows.Controls;

namespace FoodDelivery.Views.Pages.Cliente;

public partial class PedidosClientePage : Page
{
    private readonly IClienteService _service;
    private readonly AppSession _session;

    public PedidosClientePage(IClienteService service, AppSession session)
    {
        _service = service;
        _session = session;
        InitializeComponent();
        Loaded += PedidosClientePage_Loaded;
    }

    private async void PedidosClientePage_Loaded(object sender, RoutedEventArgs e)
    {
        await ReloadAsync();
    }

    private async Task ReloadAsync()
    {
        if (_session.CurrentUser is not global::FoodDelivery.Models.Users.Cliente cliente) return;
        var list = await _service.ListPedidosAsync(cliente.Id);
        PedidosItems.ItemsSource = list;
        EmptyOrdersPanel.Visibility = list.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
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

    private async void Cancel_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (_session.CurrentUser is not global::FoodDelivery.Models.Users.Cliente cliente)
                throw new FriendlyException("Sessão inválida.");

            if (sender is not FrameworkElement fe || fe.DataContext is not PedidoDto pedido)
                throw new FriendlyException("Pedido inválido.");

            if (!pedido.CanCancel)
                throw new FriendlyException("Este pedido não pode mais ser cancelado.");

            var motivo = PromptText("Cancelar pedido", "Informe o motivo do cancelamento:", "Ex.: pedi por engano");
            if (motivo is null)
                return;

            await _service.CancelarPedidoAsync(cliente.Id, pedido.Id, motivo);
            await ReloadAsync();
            MessageBox.Show("Pedido cancelado.", "Pedidos", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (FriendlyException ex)
        {
            MessageBox.Show(ex.Message, "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception)
        {
            MessageBox.Show("Não foi possível cancelar.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void CopyCode_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (sender is not FrameworkElement fe || fe.DataContext is not PedidoDto pedido)
                throw new FriendlyException("Pedido inválido.");

            Clipboard.SetText(pedido.Codigo);
            MessageBox.Show("Código copiado.", "Pedidos", MessageBoxButton.OK, MessageBoxImage.Information);
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

        var ok = new Button { Content = "Confirmar", Width = 120, Margin = new Thickness(10, 0, 0, 0) };
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
        return string.IsNullOrWhiteSpace(value) ? "Cancelado pelo cliente." : value;
    }
}
