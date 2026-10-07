namespace FilmLibrary.Services;

public interface IImageService
{
    /// <summary>Enregistre l'image et renvoie son chemin relatif (/uploads/xxx.jpg).</summary>
    Task<string> SaveAsync(IFormFile file);

    void Delete(string? relativePath);

    /// <summary>Renvoie un message d'erreur, ou null si le fichier est valide.</summary>
    string? Validate(IFormFile file);
}
