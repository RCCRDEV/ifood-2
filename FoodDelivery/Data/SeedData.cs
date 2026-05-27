using FoodDelivery.Helpers;
using FoodDelivery.Models;
using FoodDelivery.Models.Users;
using Microsoft.Extensions.DependencyInjection;

namespace FoodDelivery.Data;

public static class SeedData
{
    public static void EnsureSeeded(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<FoodDeliveryDbContext>();
        var hasher = scope.ServiceProvider.GetRequiredService<PasswordHasher>();

        if (!db.Users.Any(u => u.Email == "admin@local"))
        {
            var (hash, salt) = hasher.Hash("Admin123!");
            db.Users.Add(new Administrador
            {
                Nome = "Administrador",
                Email = "admin@local",
                PasswordHash = hash,
                PasswordSalt = salt
            });
        }

        if (!db.Restaurantes.Any())
        {
            var restaurante = new Restaurante
            {
                Nome = "Bistro Demo",
                Descricao = "Restaurante de demonstração",
                Endereco = "Centro",
                Telefone = "(00) 0000-0000"
            };

            db.Restaurantes.Add(restaurante);

            db.Produtos.AddRange(
                new Prato
                {
                    Restaurante = restaurante,
                    Nome = "Hambúrguer Artesanal",
                    Descricao = "Pão brioche, carne 160g, queijo e molho",
                    Preco = 29.90m,
                    TempoPreparoMin = 20
                },
                new Prato
                {
                    Restaurante = restaurante,
                    Nome = "Strogonoff de Frango",
                    Descricao = "Acompanha arroz e batata palha",
                    Preco = 24.50m,
                    TempoPreparoMin = 25
                },
                new Bebida
                {
                    Restaurante = restaurante,
                    Nome = "Refrigerante Lata",
                    Descricao = "350ml",
                    Preco = 6.50m,
                    VolumeMl = 350
                }
            );
        }

        db.SaveChanges();
    }
}

