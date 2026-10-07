using FilmLibrary.Models;

namespace FilmLibrary.Services;

public interface IPaymentService
{
    Task<string> CreateCheckoutSessionAsync(
        List<CartItem> cart,
        string userId,
        string deliveryAddress,
        string baseUrl);
}