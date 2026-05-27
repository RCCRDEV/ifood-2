using FoodDelivery.DTOs.Requests;
using FoodDelivery.Helpers;
using FoodDelivery.Services;
using System.Windows;

namespace FoodDelivery.Controllers;

public sealed class RegisterController
{
    private readonly IAuthService _auth;

    public RegisterController(IAuthService auth)
    {
        _auth = auth;
    }

    public async Task RegisterClienteAsync(Window window, RegisterClienteRequest request)
    {
        try
        {
            await _auth.RegisterClienteAsync(request);
            MessageBox.Show("Cadastro realizado com sucesso. Faça login para continuar.", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            window.Close();
        }
        catch (FriendlyException ex)
        {
            MessageBox.Show(ex.Message, "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception)
        {
            MessageBox.Show("Não foi possível concluir o cadastro.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public async Task RegisterMotoboyAsync(Window window, RegisterMotoboyRequest request)
    {
        try
        {
            await _auth.RegisterMotoboyAsync(request);
            MessageBox.Show("Cadastro realizado com sucesso. Faça login para continuar.", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            window.Close();
        }
        catch (FriendlyException ex)
        {
            MessageBox.Show(ex.Message, "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception)
        {
            MessageBox.Show("Não foi possível concluir o cadastro.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public async Task RegisterRestauranteAsync(Window window, RegisterRestauranteRequest request)
    {
        try
        {
            await _auth.RegisterRestauranteAsync(request);
            MessageBox.Show("Cadastro realizado com sucesso. Faça login para continuar.", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            window.Close();
        }
        catch (FriendlyException ex)
        {
            MessageBox.Show(ex.Message, "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception)
        {
            MessageBox.Show("Não foi possível concluir o cadastro.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}

