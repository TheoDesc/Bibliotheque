using FilmLibrary.Data;
using FilmLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace FilmLibrary.Services;

public class FilmService : IFilmService
{
    private readonly ApplicationDbContext _db;

    public FilmService(ApplicationDbContext db) => _db = db;

    public async Task<IReadOnlyList<Film>> GetAllAsync() =>
        await _db.Films.AsNoTracking().OrderBy(f => f.Title).ToListAsync();

    public async Task<Film?> GetByIdAsync(int id) =>
        await _db.Films.AsNoTracking().FirstOrDefaultAsync(f => f.Id == id);

    public async Task CreateAsync(Film film)
    {
        _db.Films.Add(film);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Film film)
    {
        _db.Films.Update(film);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var film = await _db.Films.FindAsync(id);
        if (film is null) return;
        _db.Films.Remove(film);
        await _db.SaveChangesAsync();
    }
}
