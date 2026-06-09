using KanGainNET.Data;
using KanGainNET.Models;
using KanGainNET.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KanGainNET.Controllers
{
    [Authorize]
    public class GrafikController : Controller
    {
        private readonly SilowniaContext _context;

        public GrafikController(SilowniaContext context) => _context = context;

        public async Task<IActionResult> Index(DateTime? data, int? lokalizacjaId)
        {
            var wybranaData = data ?? DateTime.Today;
            var lokalizacje = await _context.Lokalizacje.ToListAsync();
            var idLokalizacji = lokalizacjaId ?? lokalizacje.FirstOrDefault()?.Id;
            var startGodzina = wybranaData.Date.AddHours(8);

            var model = new GrafikViewModel
            {
                Data = wybranaData,
                Lokalizacje = lokalizacje,
                WybranaLokalizacjaId = idLokalizacji,
                Sale = await _context.Sale.Where(s => s.LokalizacjaId == idLokalizacji).ToListAsync(),
                Zajecia = await _context.Grafiki
                    .Include(g => g.ZajeciaGrupowe).Include(g => g.Sala)
                    .Where(g => g.DataStart.Date == wybranaData.Date && g.Sala.LokalizacjaId == idLokalizacji)
                    .ToListAsync(),
                GodzinySiatki = Enumerable.Range(0, 28).Select(i => startGodzina.AddMinutes(30 * i)).ToList()
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Formularz(int? id, int? salaId, DateTime? dataStart)
        {
            var model = new GrafikViewModel();
            model.ListaGodzinDropdown = GenerujListeGodzin();
            int currentLokId = 1;
            bool isStaff = User.IsInRole("Admin") || User.IsInRole("Pracownik") || User.IsInRole("Trener"); // Poprawka

            if (id.HasValue)
            {
                var wpis = await _context.Grafiki.Include(g => g.Sala).FirstOrDefaultAsync(g => g.Id == id.Value);
                if (wpis == null) return NotFound();

                model.Id = wpis.Id;
                model.SalaId = wpis.SalaId;
                model.DataStart = wpis.DataStart;
                model.DataKoniec = wpis.DataKoniec;
                model.ZajeciaGrupoweId = wpis.ZajeciaGrupoweId;
                model.PracownikId = wpis.PracownikId;
                model.CzyTylkoOdczyt = !isStaff;
                if (wpis.Sala != null) currentLokId = wpis.Sala.LokalizacjaId;
            }
            else
            {
                model.SalaId = salaId ?? 1;
                var ds = dataStart ?? DateTime.Now;
                model.DataStart = new DateTime(ds.Year, ds.Month, ds.Day, ds.Hour, (ds.Minute < 30 ? 0 : 30), 0);
                model.DataKoniec = model.DataStart.AddMinutes(30);
                model.CzyTylkoOdczyt = false;
                var s = await _context.Sale.FindAsync(model.SalaId);
                if (s != null) currentLokId = s.LokalizacjaId;
            }

            model.DostepneSale = await _context.Sale.Where(s => s.LokalizacjaId == currentLokId).ToListAsync();
            model.DostepneZajecia = await _context.ZajeciaGrupowe.ToListAsync();

            var wybraneZajecieId = model.ZajeciaGrupoweId ?? model.DostepneZajecia.FirstOrDefault()?.Id;
            if (wybraneZajecieId.HasValue)
            {
                var zajecie = await _context.ZajeciaGrupowe.FindAsync(wybraneZajecieId.Value);
                if (zajecie != null)
                {
                    model.DostepniPracownicy = await _context.Uzytkownicy
                        .Include(u => u.Profil)
                        .Where(u => u.RolaId == 3 && u.Specjalizacja != null && u.Specjalizacja.ToLower() == zajecie.Nazwa.ToLower())
                        .ToListAsync();
                }
                else
                {
                    model.DostepniPracownicy = await _context.Uzytkownicy.Include(u => u.Profil).Where(u => u.RolaId == 3).ToListAsync();
                }
            }
            else
            {
                model.DostepniPracownicy = await _context.Uzytkownicy.Include(u => u.Profil).Where(u => u.RolaId == 3).ToListAsync();
            }

            var listaTrenerowDropdown = model.DostepniPracownicy.Select(p => new
            {
                Id = p.Id,
                Text = p.Profil != null && !string.IsNullOrEmpty(p.Profil.Imie)
                    ? $"{p.Profil.Imie} {p.Profil.Nazwisko} ({p.Email})"
                    : p.Email ?? "Nieznany"
            }).ToList();

            ViewBag.PracownikId = new SelectList(listaTrenerowDropdown, "Id", "Text", model.PracownikId);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Zapisz(GrafikViewModel model)
        {
            var czyKolizjaSali = await _context.Grafiki.AnyAsync(g =>
                g.SalaId == model.SalaId &&
                g.Id != (model.Id ?? 0) &&
                g.DataStart < model.DataKoniec &&
                g.DataKoniec > model.DataStart
            );

            if (czyKolizjaSali)
            {
                ModelState.AddModelError("", "Ta sala jest już zajęta.");
            }

            if (model.PracownikId.HasValue)
            {
                var czyKolizjaPracownika = await _context.Grafiki.AnyAsync(g =>
                    g.PracownikId == model.PracownikId &&
                    g.Id != (model.Id ?? 0) &&
                    g.DataStart < model.DataKoniec &&
                    g.DataKoniec > model.DataStart
                );

                if (czyKolizjaPracownika)
                {
                    ModelState.AddModelError("", "Ten pracownik prowadzi już inne zajęcia w tym samym czasie!");
                }
            }

            if (!ModelState.IsValid)
            {
                var s = await _context.Sale.FindAsync(model.SalaId);
                int lokId = s?.LokalizacjaId ?? 1;
                model.DostepneSale = await _context.Sale.Where(x => x.LokalizacjaId == lokId).ToListAsync();
                model.DostepneZajecia = await _context.ZajeciaGrupowe.ToListAsync();
                model.ListaGodzinDropdown = GenerujListeGodzin();

                if (model.ZajeciaGrupoweId.HasValue)
                {
                    var zajecie = await _context.ZajeciaGrupowe.FindAsync(model.ZajeciaGrupoweId.Value);
                    if (zajecie != null)
                    {
                        model.DostepniPracownicy = await _context.Uzytkownicy
                            .Include(u => u.Profil)
                            .Where(u => u.RolaId == 3 && u.Specjalizacja != null && u.Specjalizacja.ToLower() == zajecie.Nazwa.ToLower())
                            .ToListAsync();
                    }
                    else
                    {
                        model.DostepniPracownicy = await _context.Uzytkownicy.Include(u => u.Profil).Where(u => u.RolaId == 3).ToListAsync();
                    }
                }
                else
                {
                    model.DostepniPracownicy = await _context.Uzytkownicy.Include(u => u.Profil).Where(u => u.RolaId == 3).ToListAsync();
                }

                var listaTrenerowDropdown = model.DostepniPracownicy.Select(p => new
                {
                    Id = p.Id,
                    Text = p.Profil != null && !string.IsNullOrEmpty(p.Profil.Imie)
                        ? $"{p.Profil.Imie} {p.Profil.Nazwisko} ({p.Email})"
                        : p.Email ?? "Nieznany"
                }).ToList();

                ViewBag.PracownikId = new SelectList(listaTrenerowDropdown, "Id", "Text", model.PracownikId);

                return View("Formularz", model);
            }

            bool isStaff = User.IsInRole("Admin") || User.IsInRole("Pracownik") || User.IsInRole("Trener");
            if (model.Id.HasValue)
            {
                if (!isStaff) return Forbid();
                var wpis = await _context.Grafiki.FindAsync(model.Id.Value);
                if (wpis != null)
                {
                    wpis.SalaId = model.SalaId;
                    wpis.DataStart = model.DataStart;
                    wpis.DataKoniec = model.DataKoniec;
                    wpis.ZajeciaGrupoweId = model.ZajeciaGrupoweId;
                    wpis.PracownikId = model.PracownikId;
                    _context.Update(wpis);
                }
            }
            else
            {
                _context.Grafiki.Add(new Grafik
                {
                    SalaId = model.SalaId,
                    DataStart = model.DataStart,
                    DataKoniec = model.DataKoniec,
                    ZajeciaGrupoweId = isStaff ? model.ZajeciaGrupoweId : null,
                    PracownikId = isStaff ? model.PracownikId : null
                });
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", new { data = model.DataStart.ToString("yyyy-MM-dd") });
        }

        [HttpGet]
        public async Task<IActionResult> PobierzTrenerowPoZajeciach(int zajeciaGrupoweId)
        {
            var zajecie = await _context.ZajeciaGrupowe.FindAsync(zajeciaGrupoweId);
            if (zajecie == null)
            {
                return Json(new List<object>());
            }

            var trenerzy = await _context.Uzytkownicy
                .Include(u => u.Profil)
                .Where(u => u.RolaId == 3 && u.Specjalizacja != null && u.Specjalizacja.ToLower() == zajecie.Nazwa.ToLower())
                .Select(u => new
                {
                    id = u.Id,
                    text = u.Profil != null && !string.IsNullOrEmpty(u.Profil.Imie)
                        ? $"{u.Profil.Imie} {u.Profil.Nazwisko} ({u.Email})"
                        : u.Email
                })
                .ToListAsync();

            return Json(trenerzy);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Pracownik, Trener")]
        public async Task<IActionResult> Usun(int id)
        {
            var wpis = await _context.Grafiki.FindAsync(id);
            if (wpis != null) { _context.Grafiki.Remove(wpis); await _context.SaveChangesAsync(); }
            return RedirectToAction("Index");
        }

        private List<string> GenerujListeGodzin()
        {
            var list = new List<string>();
            for (int h = 8; h <= 21; h++) { list.Add($"{h:D2}:00"); list.Add($"{h:D2}:30"); }
            return list;
        }
    }
}