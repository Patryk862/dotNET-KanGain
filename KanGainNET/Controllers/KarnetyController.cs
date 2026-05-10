using Microsoft.AspNetCore.Mvc;
using KanGainNET.Data;
using Stripe.Checkout;
using Microsoft.EntityFrameworkCore;

namespace KanGainNET.Controllers
{
    public class KarnetyController : Controller
    {
        private readonly SilowniaContext _context;

        public KarnetyController(SilowniaContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> KupKarnet(int id)
        {
            var karnet = await _context.TypyKarnetow.FirstOrDefaultAsync(k => k.Id == id);
            if (karnet == null) return NotFound();

            var domain = "https://localhost:7035";
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
                {
                  new SessionLineItemOptions
                  {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(karnet.Cena * 100),
                        Currency = "pln",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = karnet.Nazwa,
                        },
                    },
                    Quantity = 1,
                  },
                },
                Mode = "payment",
                SuccessUrl = domain + "/",
                CancelUrl = domain + "/",
            };

            var service = new SessionService();
            Session session = service.Create(options);

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }
    }
}