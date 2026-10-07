using System.ComponentModel.DataAnnotations;

namespace FilmLibrary.Models;

public class Film
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Le titre est obligatoire.")]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Director { get; set; }

    public int? ReleaseYear { get; set; }

    public int? DurationMinutes { get; set; }

    [StringLength(2000)]
    public string? Description { get; set; }

    [Range(0, 100000)]
    public double Prix { get; set; }

    public int Stock { get; set; }

    public string? PosterPath { get; set; }

    public int? GenreId { get; set; }

    public Genre? Genre { get; set; }
}