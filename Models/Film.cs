namespace FilmLibrary.Models;

public class Film
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Director { get; set; }
    public string? Genre { get; set; }
    public int ReleaseYear { get; set; }
    public int DurationMinutes { get; set; }
    public double Prix { get; set; }

    /// <summary>Chemin relatif dans wwwroot (ex: /uploads/abc.jpg)</summary>
    public string? PosterPath { get; set; }


}
