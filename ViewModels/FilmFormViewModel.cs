using System.ComponentModel.DataAnnotations;

namespace FilmLibrary.ViewModels;

public class FilmFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Le titre est obligatoire")]
    [StringLength(150)]
    [Display(Name = "Titre")]
    public string Title { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    [StringLength(100)]
    [Display(Name = "Réalisateur")]
    public string? Director { get; set; }

    [StringLength(50)]
    [Display(Name = "Genre")]
    public string? Genre { get; set; }

    [Range(1888, 2100, ErrorMessage = "Année invalide")]
    [Display(Name = "Année de sortie")]
    public int ReleaseYear { get; set; } = DateTime.Now.Year;

    [Range(1, 600, ErrorMessage = "Durée invalide")]
    [Display(Name = "Durée (min)")]
    public int DurationMinutes { get; set; } = 90;

    public string? ExistingPosterPath { get; set; }

    [Display(Name = "Affiche")]
    public IFormFile? Poster { get; set; }
}
