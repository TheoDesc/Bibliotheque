using FilmLibrary.Data;
using FilmLibrary.Models;
using FilmLibrary.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FilmLibrary.Controllers;

public class FilmsController : Controller
{
    private readonly IFilmService _filmService;
    private readonly ApplicationDbContext _context;

    public FilmsController(
        IFilmService filmService,
        ApplicationDbContext context)
    {
        _filmService = filmService;
        _context = context;
    }

    public async Task<IActionResult> Index(
        string? search,
        string? genre,
        double? minPrice,
        double? maxPrice)
    {
        var films = await _filmService.GetFilteredAsync(
            search,
            genre,
            minPrice,
            maxPrice);

        ViewBag.Search = search;
        ViewBag.Genre = genre;
        ViewBag.MinPrice = minPrice;
        ViewBag.MaxPrice = maxPrice;

        ViewBag.Genres = await _context.Genres
            .AsNoTracking()
            .OrderBy(g => g.Name)
            .ToListAsync();

        return View(films);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var film = await _filmService.GetByIdAsync(id.Value);

        if (film == null)
        {
            return NotFound();
        }

        return View(film);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create()
    {
        await LoadGenresAsync();

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(Film film)
    {
        if (!ModelState.IsValid)
        {
            await LoadGenresAsync(film.GenreId);

            return View(film);
        }

        await _filmService.CreateAsync(film);

        TempData["Success"] =
            "Le film a été ajouté avec succès.";

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var film = await _filmService.GetByIdAsync(id.Value);

        if (film == null)
        {
            return NotFound();
        }

        await LoadGenresAsync(film.GenreId);

        return View(film);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, Film film)
    {
        if (id != film.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            await LoadGenresAsync(film.GenreId);

            return View(film);
        }

        var existingFilm = await _filmService.GetByIdAsync(id);

        if (existingFilm == null)
        {
            return NotFound();
        }

        await _filmService.UpdateAsync(film);

        TempData["Success"] =
            "Le film a été modifié avec succès.";

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var film = await _filmService.GetByIdAsync(id.Value);

        if (film == null)
        {
            return NotFound();
        }

        return View(film);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var film = await _filmService.GetByIdAsync(id);

        if (film == null)
        {
            return NotFound();
        }

        await _filmService.DeleteAsync(id);

        TempData["Success"] =
            "Le film a été supprimé avec succès.";

        return RedirectToAction(nameof(Index));
    }

    private async Task LoadGenresAsync(int? selectedGenreId = null)
    {
        var genres = await _context.Genres
            .AsNoTracking()
            .OrderBy(g => g.Name)
            .ToListAsync();

        ViewBag.Genres = new SelectList(
            genres,
            "Id",
            "Name",
            selectedGenreId);
    }
}