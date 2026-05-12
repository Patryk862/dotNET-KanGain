using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using KanGainNET.Models;
using KanGainNET.Data;
using KanGainNET.Models.ViewModels;

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

            var model = new GrafikViewModel {
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
            bool isStaff = User.IsInRole("Admin") || User.IsInRole("Pracownik");

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
            model.DostepniPracownicy = await _context.Pracownicy.Include(p => p.Uzytkownik).ToListAsync();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Zapisz(GrafikViewModel model)
        {
            if (model.DataStart >= model.DataKoniec)
                ModelState.AddModelError("DataKoniec", "Koniec musi być po starcie.");

            if (!ModelState.IsValid)
            {
                var sala = await _context.Sale.FindAsync(model.SalaId);
                int lokId = sala?.LokalizacjaId ?? 1;
                model.DostepneSale = await _context.Sale.Where(s => s.LokalizacjaId == lokId).ToListAsync();
                model.DostepneZajecia = await _context.ZajeciaGrupowe.ToListAsync();
                model.DostepniPracownicy = await _context.Pracownicy.Include(p => p.Uzytkownik).ToListAsync();
                model.ListaGodzinDropdown = GenerujListeGodzin();
                return View("Formularz", model);
            }

            bool isStaff = User.IsInRole("Admin") || User.IsInRole("Pracownik");
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
                _context.Grafiki.Add(new Grafik {
                    SalaId = model.SalaId, DataStart = model.DataStart, DataKoniec = model.DataKoniec,
                    ZajeciaGrupoweId = isStaff ? model.ZajeciaGrupoweId : null,
                    PracownikId = isStaff ? model.PracownikId : null
                });
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", new { data = model.DataStart.ToString("yyyy-MM-dd") });
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Pracownik")]
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