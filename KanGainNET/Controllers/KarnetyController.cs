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
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

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

            var uzytkownik = await _context.Uzytkownicy
                .Include(u => u.Profil)
                .FirstOrDefaultAsync(u => u.Email == currentName || 
                u.Id.ToString() == currentName || 
                u.Profil.Imie == currentName);

            if (uzytkownik == null) 
            {
                return RedirectToAction("Index", "Home");
            }

            var subskrypcje = await _context.Subskrypcje
                .Where(s => s .UzytkownikId == uzytkownik.Id)
                .Include(s => s.TypKarnetu) 
                .Include(s => s.Platnosci) 
                .OrderByDescending(s => s.DataStartu)
                .ToListAsync();

            return View(subskrypcje);
        }

        [HttpGet]
        public async Task<IActionResult> PobierzPotwierdzeniePdf(int id)
        {
            var subskrypcja = await _context.Subskrypcje
                .Include(s => s.TypKarnetu)
                .Include(s => s.Uzytkownik)
                    .ThenInclude(u => u.Profil)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (subskrypcja == null) return NotFound();

            var platnosc = await _context.Platnosci.FirstOrDefaultAsync(p => p.SubskrypcjaId == id);
            string metodaPlatnosci = platnosc != null ? platnosc.Metoda : "Płatność online";

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12).FontFamily(Fonts.Arial));

                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("KanGain").FontSize(28).SemiBold().FontColor("#ff4500");
                            col.Item().Text("Najlepsza Kuźnia Żelastwa w Mieście").FontSize(10).FontColor(Colors.Grey.Medium);
                        });
                        row.RelativeItem().AlignRight().Text("Potwierdzenie").FontSize(24).SemiBold();
                    });

                    page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                    {
                        col.Spacing(20);

                        col.Item().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(5).Column(c =>
                        {
                            c.Item().Text("Dane wystawcy:").SemiBold();
                            c.Item().Text("Siłownia KanGain Sp. z o.o.");
                            c.Item().Text("ul. Pompy 44, 17-200 Hajnówka");
                            c.Item().Text("Telefon: +48 796 261 646");
                        });

                        col.Item().Column(c =>
                        {
                            c.Item().Text("Dane klienta:").SemiBold();
                            if (subskrypcja.Uzytkownik.Profil != null)
                            {
                                c.Item().Text($"Imię i nazwisko: {subskrypcja.Uzytkownik.Profil.Imie} {subskrypcja.Uzytkownik.Profil.Nazwisko}");
                            }
                            c.Item().Text($"Adres e-mail: {subskrypcja.Uzytkownik.Email}");
                        });

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background("#ff4500").Padding(5).Text("Rodzaj karnetu").FontColor(Colors.White).SemiBold();
                                header.Cell().Background("#ff4500").Padding(5).Text("Ważność").FontColor(Colors.White).SemiBold();
                                header.Cell().Background("#ff4500").Padding(5).Text("Płatność").FontColor(Colors.White).SemiBold();
                                header.Cell().Background("#ff4500").Padding(5).AlignRight().Text("Cena").FontColor(Colors.White).SemiBold();
                            });

                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(subskrypcja.TypKarnetu.Nazwa);
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text($"{subskrypcja.DataStartu:dd.MM.yyyy} - {subskrypcja.DataKonca:dd.MM.yyyy}");
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(metodaPlatnosci);
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignRight().Text($"{subskrypcja.TypKarnetu.Cena} zł").SemiBold();
                        });

                        col.Item().AlignRight().Text("Dziękujemy za zaufanie. Duży rośnij!").Italic().FontColor(Colors.Grey.Darken1);
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Dokument wygenerowany automatycznie w systemie KanGain | Strona ");
                        x.CurrentPageNumber();
                        x.Span(" z ");
                        x.TotalPages();
                    });
                });
            });

            byte[] pdfBytes = document.GeneratePdf();
            return File(pdfBytes, "application/pdf", $"Potwierdzenie_KanGain_{id}.pdf");
        }
    }
}