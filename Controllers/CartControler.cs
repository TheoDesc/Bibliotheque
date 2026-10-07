using System.Security.Claims;
using FilmLibrary.Data;
using FilmLibrary.Extensions;
using FilmLibrary.Models;
using FilmLibrary.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;

namespace FilmLibrary.Controllers;

public class CartController : Controller
{
    private const string CartSessionKey = "Cart";

    private readonly IFilmService _filmService;
    private readonly ApplicationDbContext _db;
    private readonly IPaymentService _paymentService;

    public CartController(
        IFilmService filmService,
        ApplicationDbContext db,
        IPaymentService paymentService)
    {
        _filmService = filmService;
        _db = db;
        _paymentService = paymentService;
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
            return NotFound();

        if (film.Stock <= 0)
        {
            TempData["Error"] = "Ce film n'est plus disponible.";
            return RedirectToAction("Index", "Films");
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
                PosterPath = film.PosterPath,
                Quantity = 1,
                Stock = film.Stock
            });
        }
        else
        {
            if (existingItem.Quantity < film.Stock)
            {
                existingItem.Quantity++;
            }
            else
            {
                TempData["Error"] =
                    "Vous avez atteint la quantité maximale disponible.";
            }
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
            SaveCart(cart);
        }

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Increase(int id)
    {
        var cart = GetCart();

        var item = cart.FirstOrDefault(x => x.FilmId == id);

        if (item != null && item.Quantity < item.Stock)
        {
            item.Quantity++;
            SaveCart(cart);
        }

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Decrease(int id)
    {
        var cart = GetCart();

        var item = cart.FirstOrDefault(x => x.FilmId == id);

        if (item != null)
        {
            item.Quantity--;

            if (item.Quantity <= 0)
                cart.Remove(item);

            SaveCart(cart);
        }

        return RedirectToAction(nameof(Index));
    }

    [Authorize]
    public IActionResult Checkout()
    {
        var cart = GetCart();

        if (!cart.Any())
            return RedirectToAction(nameof(Index));

        return View(cart);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Pay(string deliveryAddress)
    {
        var cart = GetCart();

        if (!cart.Any())
        {
            TempData["Error"] = "Votre panier est vide.";
            return RedirectToAction(nameof(Index));
        }

        if (string.IsNullOrWhiteSpace(deliveryAddress))
        {
            TempData["Error"] =
                "Veuillez renseigner une adresse de livraison.";

            return RedirectToAction(nameof(Checkout));
        }

        // Vérification du stock avant paiement
        foreach (var item in cart)
        {
            var film = await _db.Films
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == item.FilmId);

            if (film == null)
            {
                TempData["Error"] =
                    $"Le film « {item.Title} » n'existe plus.";

                return RedirectToAction(nameof(Index));
            }

            if (film.Stock < item.Quantity)
            {
                TempData["Error"] =
                    $"Stock insuffisant pour « {film.Title} ».";

                return RedirectToAction(nameof(Index));
            }
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Challenge();

        var baseUrl = $"{Request.Scheme}://{Request.Host}";

        try
        {
            var checkoutUrl =
                await _paymentService.CreateCheckoutSessionAsync(
                    cart,
                    userId,
                    deliveryAddress.Trim(),
                    baseUrl);

            return Redirect(checkoutUrl);
        }
        catch (Exception)
        {
            TempData["Error"] =
                "Impossible de créer la session de paiement Stripe.";

            return RedirectToAction(nameof(Checkout));
        }
    }

    [Authorize]
    public async Task<IActionResult> PaymentSuccess(string? session_id)
    {
        if (string.IsNullOrWhiteSpace(session_id))
        {
            TempData["Error"] =
                "Session de paiement invalide.";

            return RedirectToAction(nameof(Index));
        }

        var secretKey = HttpContext.RequestServices
            .GetRequiredService<IConfiguration>()["Stripe:SecretKey"];

        if (string.IsNullOrWhiteSpace(secretKey))
        {
            TempData["Error"] =
                "La configuration Stripe est absente.";

            return RedirectToAction(nameof(Index));
        }

        Stripe.StripeConfiguration.ApiKey = secretKey;

        var stripeService = new SessionService();

        var session = await stripeService.GetAsync(session_id);

        if (session == null || session.PaymentStatus != "paid")
        {
            TempData["Error"] =
                "Le paiement n'a pas été confirmé.";

            return RedirectToAction(nameof(Index));
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Challenge();

        // Empêche de créer deux commandes avec la même session Stripe
        var existingOrder = await _db.Orders
            .FirstOrDefaultAsync(x => x.StripeSessionId == session.Id);

        if (existingOrder != null)
        {
            return RedirectToAction(
                "Details",
                "Orders",
                new { id = existingOrder.Id });
        }

        var cart = GetCart();

        if (!cart.Any())
        {
            TempData["Error"] =
                "Le panier est vide.";

            return RedirectToAction(nameof(Index));
        }

        var deliveryAddress =
            session.Metadata.TryGetValue(
                "DeliveryAddress",
                out var address)
                ? address
                : string.Empty;

        // Vérification finale du stock
        foreach (var item in cart)
        {
            var film = await _db.Films
                .FirstOrDefaultAsync(x => x.Id == item.FilmId);

            if (film == null || film.Stock < item.Quantity)
            {
                TempData["Error"] =
                    "Le stock a changé pendant le paiement.";

                return RedirectToAction(nameof(Index));
            }
        }

        var order = new Order
        {
            UserId = userId,
            DeliveryAddress = deliveryAddress,
            CreatedAt = DateTime.UtcNow,
            Total = cart.Sum(x => x.Prix * x.Quantity),
            Status = "Confirmée",
            StripeSessionId = session.Id
        };

        foreach (var item in cart)
        {
            var film = await _db.Films
                .FirstAsync(x => x.Id == item.FilmId);

            film.Stock -= item.Quantity;

            order.Items.Add(new OrderItem
            {
                FilmId = film.Id,
                Quantity = item.Quantity,
                UnitPrice = item.Prix
            });
        }

        _db.Orders.Add(order);

        await _db.SaveChangesAsync();

        HttpContext.Session.Remove(CartSessionKey);

        return RedirectToAction(
            "Details",
            "Orders",
            new { id = order.Id });
    }

    [Authorize]
    public IActionResult PaymentCancel()
    {
        TempData["Error"] =
            "Le paiement a été annulé.";

        return RedirectToAction(nameof(Checkout));
    }

    private List<CartItem> GetCart()
    {
        return HttpContext.Session
                   .GetObject<List<CartItem>>(CartSessionKey)
               ?? new List<CartItem>();
    }

    private void SaveCart(List<CartItem> cart)
    {
        HttpContext.Session.SetObject(
            CartSessionKey,
            cart);
    }
}