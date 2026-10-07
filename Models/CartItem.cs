namespace FilmLibrary.Models;

public class CartItem
{
    public int FilmId { get; set; }

    public string Title { get; set; } = string.Empty;

    public double Prix { get; set; }

    public string? PosterPath { get; set; }

    public int Quantity { get; set; } = 1;

    public int Stock { get; set; }
}