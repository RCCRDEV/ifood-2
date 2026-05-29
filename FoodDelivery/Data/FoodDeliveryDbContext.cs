using FoodDelivery.Models;
using FoodDelivery.Models.Enums;
using FoodDelivery.Models.Users;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Data;

public sealed class FoodDeliveryDbContext : DbContext
{
    public FoodDeliveryDbContext(DbContextOptions<FoodDeliveryDbContext> options) : base(options)
    {
    }

    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Motoboy> Motoboys => Set<Motoboy>();
    public DbSet<Administrador> Administradores => Set<Administrador>();
    public DbSet<RestauranteUser> RestauranteUsers => Set<RestauranteUser>();

    public DbSet<Restaurante> Restaurantes => Set<Restaurante>();
    public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<Prato> Pratos => Set<Prato>();
    public DbSet<Bebida> Bebidas => Set<Bebida>();

    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<ItemPedido> ItensPedido => Set<ItemPedido>();

    public DbSet<FavoritoRestaurante> FavoritosRestaurantes => Set<FavoritoRestaurante>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AppUser>(e =>
        {
            e.HasIndex(x => x.Email).IsUnique();
            e.Property(x => x.Email).IsRequired();
            e.Property(x => x.Nome).IsRequired();
        });

        modelBuilder.Entity<AppUser>()
            .HasDiscriminator<string>("UserType")
            .HasValue<Cliente>("Cliente")
            .HasValue<Motoboy>("Motoboy")
            .HasValue<Administrador>("Administrador")
            .HasValue<RestauranteUser>("RestauranteUser");

        modelBuilder.Entity<RestauranteUser>(e =>
        {
            e.HasOne(x => x.Restaurante)
                .WithMany()
                .HasForeignKey(x => x.RestauranteId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Produto>()
            .HasDiscriminator<TipoProduto>("TipoProduto")
            .HasValue<Prato>(TipoProduto.Prato)
            .HasValue<Bebida>(TipoProduto.Bebida);

        modelBuilder.Entity<Produto>(e =>
        {
            e.Property(x => x.Preco).HasPrecision(18, 2);
            e.HasOne(x => x.Restaurante)
                .WithMany(r => r.Produtos)
                .HasForeignKey(x => x.RestauranteId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Pedido>(e =>
        {
            e.HasOne(x => x.Cliente)
                .WithMany(c => c.Pedidos)
                .HasForeignKey(x => x.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Restaurante)
                .WithMany(r => r.Pedidos)
                .HasForeignKey(x => x.RestauranteId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Motoboy)
                .WithMany(m => m.Entregas)
                .HasForeignKey(x => x.MotoboyId)
                .OnDelete(DeleteBehavior.Restrict);

            e.Property(x => x.MetodoPagamento).HasConversion<int>();
            e.Property(x => x.StatusPagamento).HasConversion<int>();
        });

        modelBuilder.Entity<ItemPedido>(e =>
        {
            e.HasKey(x => new { x.PedidoId, x.ProdutoId });
            e.Property(x => x.PrecoUnitario).HasPrecision(18, 2);

            e.HasOne(x => x.Pedido)
                .WithMany(p => p.Itens)
                .HasForeignKey(x => x.PedidoId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(x => x.Produto)
                .WithMany(p => p.ItensPedido)
                .HasForeignKey(x => x.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<FavoritoRestaurante>(e =>
        {
            e.HasKey(x => new { x.ClienteId, x.RestauranteId });

            e.HasOne(x => x.Cliente)
                .WithMany(c => c.Favoritos)
                .HasForeignKey(x => x.ClienteId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(x => x.Restaurante)
                .WithMany(r => r.Favoritos)
                .HasForeignKey(x => x.RestauranteId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
