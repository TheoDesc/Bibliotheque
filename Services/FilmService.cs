using FilmLibrary.Data;
using FilmLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace FilmLibrary.Services;

public class FilmService : IFilmService
{
    private readonly ApplicationDbContext _db;

    public FilmService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<Film>> GetAllAsync()
    {
        return await _db.Films
            .AsNoTracking()
            .Include(f => f.Genre)
            .OrderBy(f => f.Title)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Film>> GetFilteredAsync(
        string? search,
        string? genre,
        double? minPrice,
        double? maxPrice)
    {
        IQueryable<Film> query = _db.Films
            .AsNoTracking()
            .Include(f => f.Genre);

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();

            query = query.Where(f =>
                f.Title.Contains(search) ||
                (f.Director != null &&
                 f.Director.Contains(search)) ||
                (f.Description != null &&
                 f.Description.Contains(search)));
        }

        if (!string.IsNullOrWhiteSpace(genre))
        {
            query = query.Where(f =>
                f.Genre != null &&
                f.Genre.Name == genre);
        }

        if (minPrice.HasValue)
        {
            query = query.Where(f =>
                f.Prix >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(f =>
                f.Prix <= maxPrice.Value);
        }

        return await query
            .OrderBy(f => f.Title)
            .ToListAsync();
    }

    public async Task<Film?> GetByIdAsync(int id)
    {
        return await _db.Films
            .AsNoTracking()
            .Include(f => f.Genre)
            .FirstOrDefaultAsync(f => f.Id == id);
    }

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
        var film = await _db.Films
            .FirstOrDefaultAsync(f => f.Id == id);

        if (film == null)
        {
            return;
        }

        _db.Films.Remove(film);

        await _db.SaveChangesAsync();
    }
}