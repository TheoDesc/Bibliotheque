using FilmLibrary.Models;
using Stripe;
using Stripe.Checkout;

namespace FilmLibrary.Services;

public class PaymentService : IPaymentService
{
    private readonly IConfiguration _configuration;

    public PaymentService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<string> CreateCheckoutSessionAsync(
        List<CartItem> cart,
        string userId,
        string deliveryAddress,
        string baseUrl)
    {
        var secretKey = _configuration["Stripe:SecretKey"];

        if (string.IsNullOrWhiteSpace(secretKey))
            throw new InvalidOperationException(
                "La clé secrète Stripe n'est pas configurée.");

        StripeConfiguration.ApiKey = secretKey;

        var lineItems = cart.Select(item => new SessionLineItemOptions
        {
            PriceData = new SessionLineItemPriceDataOptions
            {
                Currency = "eur",
                UnitAmount = (long)Math.Round(item.Prix * 100),
                ProductData = new SessionLineItemPriceDataProductDataOptions
                {
                    Name = item.Title
                }
            },
            Quantity = item.Quantity
        }).ToList();

        var options = new SessionCreateOptions
        {
            Mode = "payment",

            SuccessUrl =
                $"{baseUrl}/Cart/PaymentSuccess?session_id={{CHECKOUT_SESSION_ID}}",

            CancelUrl =
                $"{baseUrl}/Cart/Checkout",

            LineItems = lineItems,

            Metadata = new Dictionary<string, string>
            {
                ["UserId"] = userId,
                ["DeliveryAddress"] = deliveryAddress
            }
        };

        var service = new SessionService();

        var session = await service.CreateAsync(options);

        return session.Url;
    }
}