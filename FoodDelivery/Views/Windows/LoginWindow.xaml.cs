using FoodDelivery.Controllers;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace FoodDelivery.Views.Windows;

public partial class LoginWindow : Window
{
    private readonly LoginController _loginController;
    private readonly IServiceProvider _services;

    public LoginWindow(LoginController loginController, IServiceProvider services)
    {
        _loginController = loginController;
        _services = services;
        InitializeComponent();
    }

    private async void Login_Click(object sender, RoutedEventArgs e)
    {
        await _loginController.LoginAsync(this, EmailTextBox.Text, PasswordBox.Password);
    }

    private void CreateAccount_Click(object sender, RoutedEventArgs e)
    {
        var window = _services.GetRequiredService<RegisterWindow>();
        window.Owner = this;
        window.ShowDialog();
    }
}

