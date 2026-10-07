namespace FilmLibrary.Services;

public class ImageService : IImageService
{
    private const long MaxBytes = 2 * 1024 * 1024;
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];

    private readonly IWebHostEnvironment _env;

    public ImageService(IWebHostEnvironment env) => _env = env;

    public string? Validate(IFormFile file)
    {
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(ext))
            return "Format non supporté (jpg, png ou webp uniquement).";
        if (file.Length > MaxBytes)
            return "L'image dépasse 2 Mo.";
        return null;
    }

    public async Task<string> SaveAsync(IFormFile file)
    {
        var folder = Path.Combine(_env.WebRootPath, "uploads");
        Directory.CreateDirectory(folder);

        var fileName = $"{Guid.NewGuid():N}{Path.GetExtension(file.FileName).ToLowerInvariant()}";
        var fullPath = Path.Combine(folder, fileName);

        await using var stream = File.Create(fullPath);
        await file.CopyToAsync(stream);

        return $"/uploads/{fileName}";
    }

    public void Delete(string? relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath)) return;

        var fullPath = Path.Combine(_env.WebRootPath, relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        if (File.Exists(fullPath)) File.Delete(fullPath);
    }
}
