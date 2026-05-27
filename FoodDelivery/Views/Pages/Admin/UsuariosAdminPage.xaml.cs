using FoodDelivery.DTOs;
using FoodDelivery.Services;
using System.Windows;
using System.Windows.Controls;

namespace FoodDelivery.Views.Pages.Admin;

public partial class UsuariosAdminPage : Page
{
    private readonly IAdminService _service;

    public UsuariosAdminPage(IAdminService service)
    {
        _service = service;
        InitializeComponent();
        Loaded += UsuariosAdminPage_Loaded;
    }

    private async void UsuariosAdminPage_Loaded(object sender, RoutedEventArgs e)
    {
        await ReloadAsync();
    }

    private async Task ReloadAsync()
    {
        Grid.ItemsSource = await _service.ListUsuariosAsync();
    }

    private async void Toggle_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (sender is not Button btn || btn.Tag is not UserDto user) return;
            await _service.ToggleUsuarioAtivoAsync(user.Id, !user.Ativo);
            await ReloadAsync();
        }
        catch (Exception)
        {
            MessageBox.Show("Não foi possível atualizar.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
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

