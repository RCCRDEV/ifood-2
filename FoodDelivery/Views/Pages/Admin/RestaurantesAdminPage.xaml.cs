using FoodDelivery.DTOs;
using FoodDelivery.Services;
using System.Windows;
using System.Windows.Controls;

namespace FoodDelivery.Views.Pages.Admin;

public partial class RestaurantesAdminPage : Page
{
    private readonly IAdminService _service;

    public RestaurantesAdminPage(IAdminService service)
    {
        _service = service;
        InitializeComponent();
        Loaded += RestaurantesAdminPage_Loaded;
    }

    private async void RestaurantesAdminPage_Loaded(object sender, RoutedEventArgs e)
    {
        await ReloadAsync();
    }

    private async Task ReloadAsync()
    {
        Grid.ItemsSource = await _service.ListRestaurantesAsync();
    }

    private async void Toggle_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (sender is not Button btn || btn.Tag is not RestauranteDto r) return;
            await _service.ToggleRestauranteAtivoAsync(r.Id, !r.Ativo);
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

