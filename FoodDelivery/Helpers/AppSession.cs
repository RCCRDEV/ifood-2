using FoodDelivery.Models.Users;

namespace FoodDelivery.Helpers;

public sealed class AppSession
{
    public AppUser? CurrentUser { get; private set; }

    public CartState Cart { get; } = new();

    public void SetUser(AppUser user)
    {
        CurrentUser = user;
    }

    public void Clear()
    {
        CurrentUser = null;
        Cart.Clear();
    }
}

public sealed class CartState
{
    public Guid? RestauranteId { get; private set; }
    public List<CartItem> Items { get; } = [];

    public void Add(Guid restauranteId, Guid produtoId, string produtoNome, decimal preco, int quantidade)
    {
        if (quantidade <= 0) return;

        if (RestauranteId is null || RestauranteId == restauranteId)
        {
            RestauranteId = restauranteId;
        }
        else
        {
            Clear();
            RestauranteId = restauranteId;
        }

        var existing = Items.FirstOrDefault(i => i.ProdutoId == produtoId);
        if (existing is null)
        {
            Items.Add(new CartItem(produtoId, produtoNome, preco, quantidade));
        }
        else
        {
            existing.Quantidade += quantidade;
        }
    }

    public void Remove(Guid produtoId)
    {
        var item = Items.FirstOrDefault(i => i.ProdutoId == produtoId);
        if (item is null) return;
        Items.Remove(item);
        if (Items.Count == 0) RestauranteId = null;
    }

    public void Clear()
    {
        Items.Clear();
        RestauranteId = null;
    }

    public decimal Total => Items.Sum(i => i.Subtotal);
}

public sealed class CartItem
{
    public CartItem(Guid produtoId, string produtoNome, decimal preco, int quantidade)
    {
        ProdutoId = produtoId;
        ProdutoNome = produtoNome;
        Preco = preco;
        Quantidade = quantidade;
    }

    public Guid ProdutoId { get; }
    public string ProdutoNome { get; }
    public decimal Preco { get; }
    public int Quantidade { get; set; }
    public decimal Subtotal => Preco * Quantidade;
}
