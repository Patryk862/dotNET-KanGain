using KanGainNET.Data;
using KanGainNET.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace KanGainNET.Controllers
{
    [Authorize]
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

            var successUrl = Url.Action("Sukces", "Karnety", new { karnetId = id }, Request.Scheme);
            var cancelUrl = Url.Action("Index", "Home", null, Request.Scheme);

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
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl,
            };

            var service = new SessionService();
            Session session = service.Create(options);

            return Redirect(session.Url);
        }

        [HttpGet]
        public async Task<IActionResult> Sukces(int karnetId)
        {
            var typKarnetu = await _context.TypyKarnetow.FindAsync(karnetId);
            if (typKarnetu == null) return RedirectToAction("Index", "Home");

            // POBIERANIE UŻYTKOWNIKA (Tryb Rozszerzony)
            var currentName = User.Identity?.Name;
            var uzytkownik = await _context.Uzytkownicy
                .Include(u => u.Profil)
                .FirstOrDefaultAsync(u => u.Email == currentName || 
                                         u.Id.ToString() == currentName || 
                                         u.Profil.Imie == currentName);

            if (uzytkownik != null)
            {
                var ostatniKarnet = await _context.Subskrypcje
                    .Where(s => s.UzytkownikId == uzytkownik.Id)
                    .OrderByDescending(s => s.DataKonca)
                    .FirstOrDefaultAsync();

                DateTime dataStartuNowego = (ostatniKarnet != null && ostatniKarnet.DataKonca > DateTime.Now) 
                    ? ostatniKarnet.DataKonca 
                    : DateTime.Now;

                var nowaSubskrypcja = new Subskrypcja
                {
                    DataStartu = dataStartuNowego,
                    DataKonca = dataStartuNowego.AddDays(typKarnetu.CzasTrwaniaDni),
                    UzytkownikId = uzytkownik.Id,
                    TypKarnetuId = typKarnetu.Id
                };

                _context.Subskrypcje.Add(nowaSubskrypcja);
                await _context.SaveChangesAsync();

                _context.Platnosci.Add(new Platnosc
                {
                    Kwota = typKarnetu.Cena,
                    DataPlatnosci = DateTime.Now,
                    Metoda = "Stripe",
                    SubskrypcjaId = nowaSubskrypcja.Id
                });
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("MojeKarnety");
        }

        [HttpGet]
        public async Task<IActionResult> MojeKarnety()
        {
            var currentName = User.Identity?.Name;
            
            if (string.IsNullOrEmpty(currentName)) 
                return RedirectToAction("Logowanie", "Konto");

            // SZUKAMY: po emailu LUB po imieniu z profilu (bo to masz w Identity)
            var uzytkownik = await _context.Uzytkownicy
                .Include(u => u.Profil)
                .FirstOrDefaultAsync(u => u.Email == currentName || 
                                         u.Id.ToString() == currentName || 
                                         u.Profil.Imie == currentName);

            if (uzytkownik == null) 
            {
                // Jeśli nie znaleźliśmy użytkownika, wracamy do bazy (tu mógł być błąd przekierowania)
                return RedirectToAction("Index", "Home");
            }

            var subskrypcje = await _context.Subskrypcje
                .Where(s => s.UzytkownikId == uzytkownik.Id)
                .Include(s => s.TypKarnetu)
                .Include(s => s.Platnosci) 
                .OrderByDescending(s => s.DataStartu)
                .ToListAsync();

            return View(subskrypcje);
        }
    }
}