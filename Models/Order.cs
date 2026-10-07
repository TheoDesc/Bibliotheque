namespace FilmLibrary.Models;

public class Order
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public string DeliveryAddress { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public double Total { get; set; }

    public string Status { get; set; } = "Confirmée";

    public string? StripeSessionId { get; set; }

    public List<OrderItem> Items { get; set; } = new();
}