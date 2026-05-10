using KanGainNET.Data;
using KanGainNET.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;

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
                SuccessUrl = domain + "/Karnety/Sukces?karnetId=" + id,
                CancelUrl = domain + "/",
            };

            var service = new SessionService();
            Session session = service.Create(options);

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        [HttpGet]
        public async Task<IActionResult> Sukces(int karnetId)
        {
            var typKarnetu = await _context.TypyKarnetow.FindAsync(karnetId);
            if (typKarnetu == null) return RedirectToAction("Index", "Home");

            var uzytkownik = await _context.Uzytkownicy
                .Include(u => u.Profil) 
                .FirstOrDefaultAsync(u => u.Email == User.Identity.Name || u.Profil.Imie == User.Identity.Name);

            if (uzytkownik != null)
            {
                var nowaSubskrypcja = new Subskrypcja
                {
                    DataStartu = DateTime.Now,
                    DataKonca = DateTime.Now.AddDays(typKarnetu.CzasTrwaniaDni),
                    UzytkownikId = uzytkownik.Id,
                    TypKarnetuId = typKarnetu.Id
                };

                _context.Subskrypcje.Add(nowaSubskrypcja);
                await _context.SaveChangesAsync();

                var nowaPlatnosc = new Platnosc
                {
                    Kwota = typKarnetu.Cena,
                    DataPlatnosci = DateTime.Now,
                    Metoda = "Stripe",
                    SubskrypcjaId = nowaSubskrypcja.Id
                };

                _context.Platnosci.Add(nowaPlatnosc);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Home");
        }
    }
}