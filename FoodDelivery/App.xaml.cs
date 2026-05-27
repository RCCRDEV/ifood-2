using FoodDelivery.Controllers;
using FoodDelivery.Data;
using FoodDelivery.Helpers;
using FoodDelivery.Repositories;
using FoodDelivery.Services;
using FoodDelivery.Views.Pages.Admin;
using FoodDelivery.Views.Pages.Cliente;
using FoodDelivery.Views.Pages.Motoboy;
using FoodDelivery.Views.Pages.Restaurante;
using FoodDelivery.Views.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace FoodDelivery;

public partial class App : Application
{
    private IHost? _host;

    private void Application_Startup(object sender, StartupEventArgs e)
    {
        ConfigureGlobalExceptionHandlers();

        try
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration(config =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddDbContext<FoodDeliveryDbContext>(options =>
                        options.UseSqlServer(context.Configuration.GetConnectionString("Default")));

                    services.AddSingleton<AppSession>();

                    services.AddScoped<PasswordHasher>();

                    services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
                    services.AddScoped<IUserRepository, UserRepository>();
                    services.AddScoped<IRestauranteRepository, RestauranteRepository>();
                    services.AddScoped<IProdutoRepository, ProdutoRepository>();
                    services.AddScoped<IPedidoRepository, PedidoRepository>();

                    services.AddScoped<IAuthService, AuthService>();
                    services.AddScoped<IClienteService, ClienteService>();
                    services.AddScoped<IRestauranteService, RestauranteService>();
                    services.AddScoped<IMotoboyService, MotoboyService>();
                    services.AddScoped<IAdminService, AdminService>();

                    services.AddScoped<LoginController>();
                    services.AddScoped<RegisterController>();
                    services.AddScoped<ShellController>();

                    services.AddTransient<LoginWindow>();
                    services.AddTransient<RegisterWindow>();
                    services.AddTransient<ShellWindow>();

                    services.AddTransient<RestaurantesPage>();
                    services.AddTransient<CarrinhoPage>();
                    services.AddTransient<PedidosClientePage>();
                    services.AddTransient<PerfilClientePage>();

                    services.AddTransient<ProdutosPage>();
                    services.AddTransient<PedidosRestaurantePage>();

                    services.AddTransient<EntregasDisponiveisPage>();
                    services.AddTransient<HistoricoEntregasPage>();

                    services.AddTransient<DashboardAdminPage>();
                    services.AddTransient<UsuariosAdminPage>();
                    services.AddTransient<RestaurantesAdminPage>();
                    services.AddTransient<PedidosAdminPage>();
                })
                .Build();

            using (var scope = _host.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<FoodDeliveryDbContext>();
                db.Database.EnsureCreated();
                SeedData.EnsureSeeded(scope.ServiceProvider);
            }

            var loginWindow = _host.Services.GetRequiredService<LoginWindow>();
            loginWindow.Show();
        }
        catch (Exception ex)
        {
            ShowStartupError(ex);
            Shutdown(1);
        }
    }

    private async void Application_Exit(object sender, ExitEventArgs e)
    {
        if (_host is not null)
        {
            await _host.StopAsync();
            _host.Dispose();
        }
    }

    private void ConfigureGlobalExceptionHandlers()
    {
        DispatcherUnhandledException += (_, args) =>
        {
            ShowStartupError(args.Exception);
            args.Handled = true;
        };

        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
        {
            if (args.ExceptionObject is Exception ex)
                ShowStartupError(ex);
        };

        TaskScheduler.UnobservedTaskException += (_, args) =>
        {
            ShowStartupError(args.Exception);
            args.SetObserved();
        };
    }

    private static void ShowStartupError(Exception ex)
    {
        try
        {
            var message = BuildFriendlyStartupMessage(ex);
            MessageBox.Show(message, "Erro ao iniciar o sistema", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch
        {
            Debug.WriteLine(ex);
        }
    }

    private static string BuildFriendlyStartupMessage(Exception ex)
    {
        var sb = new StringBuilder();
        sb.AppendLine("O aplicativo fechou ao iniciar.");
        sb.AppendLine();
        sb.AppendLine("Causas comuns:");
        sb.AppendLine("- SQL Server LocalDB não instalado (connection string padrão usa (localdb)\\MSSQLLocalDB).");
        sb.AppendLine("- Connection string incorreta no appsettings.json.");
        sb.AppendLine();
        sb.AppendLine("Detalhes:");
        sb.AppendLine(ex.Message);
        return sb.ToString();
    }
}
