using FilmLibrary.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FilmLibrary.Controllers;

[Authorize]
public class OrdersController : Controller
{
    private readonly ApplicationDbContext _db;

    public OrdersController(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirst(
            System.Security.Claims.ClaimTypes.NameIdentifier
        )!.Value;

        var orders = await _db.Orders
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .Include(x => x.Items)
            .ToListAsync();

        return View(orders);
    }

    public async Task<IActionResult> Details(int id)
    {
        var userId = User.FindFirst(
            System.Security.Claims.ClaimTypes.NameIdentifier
        )!.Value;

        var order = await _db.Orders
            .AsNoTracking()
            .Include(x => x.Items)
            .ThenInclude(x => x.Film)
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.UserId == userId);

        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }
}