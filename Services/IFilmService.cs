using FilmLibrary.Models;

namespace FilmLibrary.Services;

public interface IFilmService
{
    Task<IReadOnlyList<Film>> GetAllAsync();

    Task<IReadOnlyList<Film>> GetFilteredAsync(
        string? search,
        string? genre,
        double? minPrice,
        double? maxPrice);

    Task<Film?> GetByIdAsync(int id);

    Task CreateAsync(Film film);

    Task UpdateAsync(Film film);

    Task DeleteAsync(int id);
}