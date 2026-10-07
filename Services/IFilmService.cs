using FilmLibrary.Models;

namespace FilmLibrary.Services;

public interface IFilmService
{
    Task<IReadOnlyList<Film>> GetAllAsync();
    Task<Film?> GetByIdAsync(int id);
    Task CreateAsync(Film film);
    Task UpdateAsync(Film film);
    Task DeleteAsync(int id);
}
