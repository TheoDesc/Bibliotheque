using FilmLibrary.Data;
using FilmLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FilmLibrary.Controllers;

[Authorize(Roles = "Admin")]
public class GenresController : Controller
{
    private readonly ApplicationDbContext _context;

    public GenresController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Genres
    public async Task<IActionResult> Index()
    {
        var genres = await _context.Genres
            .AsNoTracking()
            .OrderBy(g => g.Name)
            .ToListAsync();

        return View(genres);
    }

    // GET: Genres/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Genres/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Genre genre)
    {
        if (string.IsNullOrWhiteSpace(genre.Name))
        {
            ModelState.AddModelError(
                nameof(genre.Name),
                "Le nom du genre est obligatoire.");
        }

        if (await _context.Genres.AnyAsync(
                g => g.Name.ToLower() == genre.Name.ToLower()))
        {
            ModelState.AddModelError(
                nameof(genre.Name),
                "Ce genre existe déjà.");
        }

        if (!ModelState.IsValid)
        {
            return View(genre);
        }

        genre.Name = genre.Name.Trim();

        _context.Genres.Add(genre);

        await _context.SaveChangesAsync();

        TempData["Success"] = "Le genre a été ajouté avec succès.";

        return RedirectToAction(nameof(Index));
    }

    // GET: Genres/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var genre = await _context.Genres
            .FirstOrDefaultAsync(g => g.Id == id.Value);

        if (genre == null)
        {
            return NotFound();
        }

        return View(genre);
    }

    // POST: Genres/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Genre genre)
    {
        if (id != genre.Id)
        {
            return NotFound();
        }

        if (string.IsNullOrWhiteSpace(genre.Name))
        {
            ModelState.AddModelError(
                nameof(genre.Name),
                "Le nom du genre est obligatoire.");
        }

        if (await _context.Genres.AnyAsync(
                g => g.Id != genre.Id &&
                     g.Name.ToLower() == genre.Name.ToLower()))
        {
            ModelState.AddModelError(
                nameof(genre.Name),
                "Ce genre existe déjà.");
        }

        if (!ModelState.IsValid)
        {
            return View(genre);
        }

        genre.Name = genre.Name.Trim();

        try
        {
            _context.Genres.Update(genre);

            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Genres.AnyAsync(g => g.Id == id))
            {
                return NotFound();
            }

            throw;
        }

        TempData["Success"] = "Le genre a été modifié avec succès.";

        return RedirectToAction(nameof(Index));
    }

    // GET: Genres/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var genre = await _context.Genres
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.Id == id.Value);

        if (genre == null)
        {
            return NotFound();
        }

        return View(genre);
    }

    // POST: Genres/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var genre = await _context.Genres
            .FirstOrDefaultAsync(g => g.Id == id);

        if (genre == null)
        {
            return NotFound();
        }

        _context.Genres.Remove(genre);

        await _context.SaveChangesAsync();

        TempData["Success"] = "Le genre a été supprimé avec succès.";

        return RedirectToAction(nameof(Index));
    }
}