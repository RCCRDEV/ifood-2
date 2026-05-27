using FoodDelivery.Helpers;
using FoodDelivery.Models.Users;
using FoodDelivery.Views.Pages.Admin;
using FoodDelivery.Views.Pages.Cliente;
using FoodDelivery.Views.Pages.Motoboy;
using FoodDelivery.Views.Pages.Restaurante;
using FoodDelivery.Views.Windows;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;

namespace FoodDelivery.Controllers;

public sealed class ShellController
{
    private readonly AppSession _session;
    private readonly IServiceProvider _services;

    public ShellController(AppSession session, IServiceProvider services)
    {
        _session = session;
        _services = services;
    }

    public string GetUserTitle()
    {
        var user = _session.CurrentUser;
        if (user is null) return string.Empty;
        return $"{user.Nome} • {user.GetType().Name}";
    }

    public IReadOnlyList<ShellNavItem> GetNavItems()
    {
        return _session.CurrentUser switch
        {
            Cliente => new[]
            {
                new ShellNavItem("Restaurantes", "", "cliente.restaurantes"),
                new ShellNavItem("Carrinho", "", "cliente.carrinho"),
                new ShellNavItem("Pedidos", "", "cliente.pedidos"),
                new ShellNavItem("Perfil", "", "cliente.perfil")
            },
            RestauranteUser => new[]
            {
                new ShellNavItem("Produtos", "", "rest.produtos"),
                new ShellNavItem("Pedidos", "", "rest.pedidos")
            },
            Motoboy => new[]
            {
                new ShellNavItem("Disponíveis", "", "moto.disponiveis"),
                new ShellNavItem("Histórico", "", "moto.historico")
            },
            Administrador => new[]
            {
                new ShellNavItem("Dashboard", "", "admin.dashboard"),
                new ShellNavItem("Usuários", "", "admin.usuarios"),
                new ShellNavItem("Restaurantes", "", "admin.restaurantes"),
                new ShellNavItem("Pedidos", "", "admin.pedidos")
            },
            _ => Array.Empty<ShellNavItem>()
        };
    }

    public void Navigate(Frame frame, string route)
    {
        Page page = route switch
        {
            "cliente.restaurantes" => _services.GetRequiredService<RestaurantesPage>(),
            "cliente.carrinho" => _services.GetRequiredService<CarrinhoPage>(),
            "cliente.pedidos" => _services.GetRequiredService<PedidosClientePage>(),
            "cliente.perfil" => _services.GetRequiredService<PerfilClientePage>(),
            "rest.produtos" => _services.GetRequiredService<ProdutosPage>(),
            "rest.pedidos" => _services.GetRequiredService<PedidosRestaurantePage>(),
            "moto.disponiveis" => _services.GetRequiredService<EntregasDisponiveisPage>(),
            "moto.historico" => _services.GetRequiredService<HistoricoEntregasPage>(),
            "admin.dashboard" => _services.GetRequiredService<DashboardAdminPage>(),
            "admin.usuarios" => _services.GetRequiredService<UsuariosAdminPage>(),
            "admin.restaurantes" => _services.GetRequiredService<RestaurantesAdminPage>(),
            "admin.pedidos" => _services.GetRequiredService<PedidosAdminPage>(),
            _ => _services.GetRequiredService<DashboardAdminPage>()
        };

        frame.Navigate(page);
    }

    public void Logout(Window shellWindow)
    {
        _session.Clear();
        var login = _services.GetRequiredService<LoginWindow>();
        login.Show();
        shellWindow.Close();
    }
}

public sealed record ShellNavItem(string Label, string IconGlyph, string Route);

