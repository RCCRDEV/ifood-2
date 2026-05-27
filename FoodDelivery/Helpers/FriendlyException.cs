namespace FoodDelivery.Helpers;

public sealed class FriendlyException : Exception
{
    public FriendlyException(string message) : base(message)
    {
    }
}

