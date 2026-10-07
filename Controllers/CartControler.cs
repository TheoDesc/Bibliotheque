using FilmLibrary.Extensions;
using FilmLibrary.Models;
using FilmLibrary.Services;
using Microsoft.AspNetCore.Mvc;

namespace FilmLibrary.Controllers;

public class CartController : Controller
{
    private const string CartSessionKey = "Cart";

    private readonly IFilmService _filmService;

    public CartController(IFilmService filmService)
    {
        _filmService = filmService;
    }

    public IActionResult Index()
    {
        var cart = GetCart();

        return View(cart);
    }

    public async Task<IActionResult> Add(int id)
    {
        var film = await _filmService.GetByIdAsync(id);

        if (film == null)
        {
            return NotFound();
        }

        var cart = GetCart();

        var existingItem = cart.FirstOrDefault(x => x.FilmId == id);

        if (existingItem == null)
        {
            cart.Add(new CartItem
            {
                FilmId = film.Id,
                Title = film.Title,
                Prix = film.Prix,
                PosterPath = film.PosterPath
            });
        }

        SaveCart(cart);

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Remove(int id)
    {
        var cart = GetCart();

        var item = cart.FirstOrDefault(x => x.FilmId == id);

        if (item != null)
        {
            cart.Remove(item);
        }

        SaveCart(cart);

        return RedirectToAction(nameof(Index));
    }
    public IActionResult Checkout()
    {
        var cart = GetCart();

        if (!cart.Any())
        {
            return RedirectToAction(nameof(Index));
        }

        return View(cart);
    }
    public IActionResult PaymentSuccess()
    {
        var cart = GetCart();

        if (!cart.Any())
        {
            return RedirectToAction(nameof(Index));
        }

        return View();
    }

    private List<CartItem> GetCart()
    {
        return HttpContext.Session
            .GetObject<List<CartItem>>(CartSessionKey)
            ?? new List<CartItem>();
    }

    private void SaveCart(List<CartItem> cart)
    {
        HttpContext.Session.SetObject(CartSessionKey, cart);
    }
}