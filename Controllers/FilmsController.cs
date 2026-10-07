using FilmLibrary.Data;
using FilmLibrary.Models;
using FilmLibrary.Services;
using FilmLibrary.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FilmLibrary.Controllers;

[Authorize]
public class FilmsController : Controller
{
    private readonly IFilmService _films;
    private readonly IImageService _images;

    public FilmsController(IFilmService films, IImageService images)
    {
        _films = films;
        _images = images;
    }

    public async Task<IActionResult> Index() => View(await _films.GetAllAsync());

    public async Task<IActionResult> Details(int id)
    {
        var film = await _films.GetByIdAsync(id);
        return film is null ? NotFound() : View(film);
    }

    // ---------- CREATE (Admin) ----------
    [Authorize(Roles = DbInitializer.AdminRole)]
    public IActionResult Create() => View(new FilmFormViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = DbInitializer.AdminRole)]
    public async Task<IActionResult> Create(FilmFormViewModel vm)
    {
        if (!await ValidatePosterAsync(vm)) return View(vm);

        var film = new Film();
        Map(vm, film);
        if (vm.Poster is { Length: > 0 })
            film.PosterPath = await _images.SaveAsync(vm.Poster);

        await _films.CreateAsync(film);
        return RedirectToAction(nameof(Index));
    }

    // ---------- EDIT (Admin) ----------
    [Authorize(Roles = DbInitializer.AdminRole)]
    public async Task<IActionResult> Edit(int id)
    {
        var film = await _films.GetByIdAsync(id);
        if (film is null) return NotFound();

        return View(new FilmFormViewModel
        {
            Id = film.Id,
            Title = film.Title,
            Description = film.Description,
            Director = film.Director,
            Genre = film.Genre,
            ReleaseYear = film.ReleaseYear,
            DurationMinutes = film.DurationMinutes,
            ExistingPosterPath = film.PosterPath
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = DbInitializer.AdminRole)]
    public async Task<IActionResult> Edit(int id, FilmFormViewModel vm)
    {
        if (id != vm.Id) return BadRequest();

        var film = await _films.GetByIdAsync(id);
        if (film is null) return NotFound();

        if (!await ValidatePosterAsync(vm))
        {
            vm.ExistingPosterPath = film.PosterPath;
            return View(vm);
        }

        Map(vm, film);
        if (vm.Poster is { Length: > 0 })
        {
            _images.Delete(film.PosterPath);
            film.PosterPath = await _images.SaveAsync(vm.Poster);
        }

        await _films.UpdateAsync(film);
        return RedirectToAction(nameof(Index));
    }

    // ---------- DELETE (Admin) ----------
    [Authorize(Roles = DbInitializer.AdminRole)]
    public async Task<IActionResult> Delete(int id)
    {
        var film = await _films.GetByIdAsync(id);
        return film is null ? NotFound() : View(film);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    [Authorize(Roles = DbInitializer.AdminRole)]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var film = await _films.GetByIdAsync(id);
        if (film is not null)
        {
            _images.Delete(film.PosterPath);
            await _films.DeleteAsync(id);
        }
        return RedirectToAction(nameof(Index));
    }

    // ---------- helpers ----------
    private Task<bool> ValidatePosterAsync(FilmFormViewModel vm)
    {
        if (vm.Poster is { Length: > 0 })
        {
            var error = _images.Validate(vm.Poster);
            if (error is not null) ModelState.AddModelError(nameof(vm.Poster), error);
        }
        return Task.FromResult(ModelState.IsValid);
    }

    private static void Map(FilmFormViewModel vm, Film film)
    {
        film.Title = vm.Title;
        film.Description = vm.Description;
        film.Director = vm.Director;
        film.Genre = vm.Genre;
        film.ReleaseYear = vm.ReleaseYear;
        film.DurationMinutes = vm.DurationMinutes;
    }
}
