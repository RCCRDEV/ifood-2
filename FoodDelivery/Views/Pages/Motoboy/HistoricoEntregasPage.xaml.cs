using FoodDelivery.DTOs;
using FoodDelivery.Helpers;
using FoodDelivery.Models.Enums;
using FoodDelivery.Services;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace FoodDelivery.Views.Pages.Motoboy;

public partial class HistoricoEntregasPage : Page
{
    private readonly IMotoboyService _service;
    private readonly AppSession _session;

    public HistoricoEntregasPage(IMotoboyService service, AppSession session)
    {
        _service = service;
        _session = session;
        InitializeComponent();
        Loaded += HistoricoEntregasPage_Loaded;
    }

    private async void HistoricoEntregasPage_Loaded(object sender, RoutedEventArgs e)
    {
        await ReloadAsync();
    }

    private async Task ReloadAsync()
    {
        if (_session.CurrentUser is not global::FoodDelivery.Models.Users.Motoboy motoboy) return;
        Grid.ItemsSource = await _service.HistoricoAsync(motoboy.Id);
    }

    private async void Delivered_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (_session.CurrentUser is not global::FoodDelivery.Models.Users.Motoboy motoboy)
                throw new FriendlyException("Sessão inválida.");

            if (Grid.SelectedItem is not PedidoDto pedido)
                throw new FriendlyException("Selecione uma entrega.");

            await _service.MarcarComoEntregueAsync(pedido.Id, motoboy.Id);
            await ReloadAsync();
            MessageBox.Show("Status atualizado para Entregue.", "Entregas", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (FriendlyException ex)
        {
            MessageBox.Show(ex.Message, "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception)
        {
            MessageBox.Show("Não foi possível atualizar.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
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

    private async void NotDelivered_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (_session.CurrentUser is not global::FoodDelivery.Models.Users.Motoboy motoboy)
                throw new FriendlyException("Sessão inválida.");

            if (Grid.SelectedItem is not PedidoDto pedido)
                throw new FriendlyException("Selecione uma entrega.");

            var motivo = PromptText("Não entregue", "Explique o que aconteceu:", "Ex.: cliente não atendeu");
            if (motivo is null)
                return;
            await _service.ReportarNaoEntregaAsync(pedido.Id, motoboy.Id, motivo);
            await ReloadAsync();
            MessageBox.Show("Entrega marcada como não concluída.", "Entregas", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (FriendlyException ex)
        {
            MessageBox.Show(ex.Message, "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
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

        var root = new System.Windows.Controls.Grid { Margin = new Thickness(18) };
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
        System.Windows.Controls.Grid.SetRow(box, 1);
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
        System.Windows.Controls.Grid.SetRow(buttons, 2);
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
        return string.IsNullOrWhiteSpace(value) ? "Entrega não concluída." : value;
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
