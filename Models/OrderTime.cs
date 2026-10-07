namespace FilmLibrary.Models;

public class OrderItem
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public Order Order { get; set; } = null!;

    public int FilmId { get; set; }

    public Film Film { get; set; } = null!;

    public int Quantity { get; set; }

    public double UnitPrice { get; set; }
}