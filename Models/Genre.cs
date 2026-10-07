using System.ComponentModel.DataAnnotations;

namespace FilmLibrary.Models;

public class Genre
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Le nom du genre est obligatoire.")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public ICollection<Film> Films { get; set; } = new List<Film>();
}