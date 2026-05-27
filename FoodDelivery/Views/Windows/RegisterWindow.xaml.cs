using FoodDelivery.Controllers;
using FoodDelivery.DTOs.Requests;
using System.Windows;

namespace FoodDelivery.Views.Windows;

public partial class RegisterWindow : Window
{
    private readonly RegisterController _controller;

    public RegisterWindow(RegisterController controller)
    {
        _controller = controller;
        InitializeComponent();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private async void CreateCliente_Click(object sender, RoutedEventArgs e)
    {
        var request = new RegisterClienteRequest(
            CliNome.Text,
            CliEmail.Text,
            CliSenha.Password,
            CliTelefone.Text,
            CliEndereco.Text
        );
        await _controller.RegisterClienteAsync(this, request);
    }

    private async void CreateRestaurante_Click(object sender, RoutedEventArgs e)
    {
        var request = new RegisterRestauranteRequest(
            ResNomeUsuario.Text,
            ResEmail.Text,
            ResSenha.Password,
            ResNomeRestaurante.Text,
            ResDescricao.Text,
            ResEndereco.Text,
            ResTelefone.Text
        );
        await _controller.RegisterRestauranteAsync(this, request);
    }

    private async void CreateMotoboy_Click(object sender, RoutedEventArgs e)
    {
        var request = new RegisterMotoboyRequest(
            MotNome.Text,
            MotEmail.Text,
            MotSenha.Password,
            MotPlaca.Text
        );
        await _controller.RegisterMotoboyAsync(this, request);
    }
}

