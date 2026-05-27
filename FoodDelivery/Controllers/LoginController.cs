using FoodDelivery.Helpers;
using FoodDelivery.Services;
using FoodDelivery.Views.Windows;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace FoodDelivery.Controllers;

public sealed class LoginController
{
    private readonly IAuthService _auth;
    private readonly AppSession _session;
    private readonly IServiceProvider _services;

    public LoginController(IAuthService auth, AppSession session, IServiceProvider services)
    {
        _auth = auth;
        _session = session;
        _services = services;
    }

    public async Task LoginAsync(Window loginWindow, string email, string password)
    {
        try
        {
            var user = await _auth.LoginAsync(email, password);
            _session.SetUser(user);

            var shell = _services.GetRequiredService<ShellWindow>();
            shell.Show();
            loginWindow.Close();
        }
        catch (FriendlyException ex)
        {
            MessageBox.Show(ex.Message, "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception)
        {
            MessageBox.Show("Não foi possível realizar o login.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}

