using FoodDelivery.Helpers;
using FoodDelivery.Services;
using System.Windows;
using System.Windows.Controls;

namespace FoodDelivery.Views.Pages.Cliente;

public partial class PerfilClientePage : Page
{
    private readonly AppSession _session;
    private readonly IAuthService _auth;

    public PerfilClientePage(AppSession session, IAuthService auth)
    {
        _session = session;
        _auth = auth;
        InitializeComponent();
        Loaded += PerfilClientePage_Loaded;
    }

    private void PerfilClientePage_Loaded(object sender, RoutedEventArgs e)
    {
        if (_session.CurrentUser is not global::FoodDelivery.Models.Users.Cliente cliente) return;
        NomeTextBox.Text = cliente.Nome;
        EmailTextBox.Text = cliente.Email;
        TelefoneTextBox.Text = cliente.Telefone ?? string.Empty;
        EnderecoTextBox.Text = cliente.Endereco ?? string.Empty;
    }

    private async void Save_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (_session.CurrentUser is not global::FoodDelivery.Models.Users.Cliente cliente) return;

            await _auth.UpdateClientePerfilAsync(cliente.Id, NomeTextBox.Text, TelefoneTextBox.Text, EnderecoTextBox.Text);
            MessageBox.Show("Perfil atualizado com sucesso.", "Perfil", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (FriendlyException ex)
        {
            MessageBox.Show(ex.Message, "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception)
        {
            MessageBox.Show("Não foi possível salvar.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
