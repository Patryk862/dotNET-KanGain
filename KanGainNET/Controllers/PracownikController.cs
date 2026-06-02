using KanGainNET.Data;
using KanGainNET.Models;
using KanGainNET.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace KanGainNET.Controllers
{
    [Authorize(Roles = "Trener")]
    public class PracownikController : Controller
    {

        private readonly SilowniaContext _context;

        public PracownikController(SilowniaContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Grafik()
        {
            var uzytkownikIdKlaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(uzytkownikIdKlaim) || !int.TryParse(uzytkownikIdKlaim, out int loggedInUserId))
            {
                return RedirectToAction("Login", "Account");
            }

            var pracownik = await _context.Pracownicy
                .FirstOrDefaultAsync(p => p.UzytkownikId == loggedInUserId);


            var grafikPracownika = await _context.Grafiki
                .Include(g => g.ZajeciaGrupowe) 
                .Include(g => g.Sala)          
                .Where(g => g.PracownikId == pracownik.Id)
                .OrderBy(g => g.DataStart)
                .ToListAsync();

            return View(grafikPracownika);
        }

        public async Task<IActionResult> PokazWGrafikuGlownym(int id)
        {
            var wpis = await _context.Grafiki
                .Include(g => g.Sala)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (wpis == null)
            {
                return NotFound();
            }

            return RedirectToAction("Index", "Grafik", new
            {
                data = wpis.DataStart.ToString("yyyy-MM-dd"),
                lokalizacjaId = wpis.Sala?.LokalizacjaId
            });
        }

        public async Task<IActionResult> PlanyTreningowe()
        {
            var uzytkownikIdKlaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(uzytkownikIdKlaim) || !int.TryParse(uzytkownikIdKlaim, out int loggedInUserId))
            {
                return RedirectToAction("Login", "Account");
            }

            var pracownik = await _context.Pracownicy
                .FirstOrDefaultAsync(p => p.UzytkownikId == loggedInUserId);

            if (pracownik == null)
            {
                return Forbid();
            }

            var planyPracownika = await _context.PlanyTreningowe
                .Include(p => p.Uzytkownik).ThenInclude(u => u.Profil)
                .Where(p => p.PracownikId == pracownik.Id)
                .OrderByDescending(p => p.DataStworzenia)
                .ToListAsync();

            return View(planyPracownika);
        }

        [HttpGet]
        public async Task<IActionResult> StworzPlan(int? id)
        {
            var uzytkownicy = await _context.Uzytkownicy
                .Include(u => u.Profil)
                .Include(u => u.Rola)
                .Include(u => u.Pracownik)
                .Where(u => u.Pracownik == null) 
                .Where(u => u.Rola == null || (!u.Rola.Nazwa.ToLower().Contains("admin") && !u.Rola.Nazwa.ToLower().Contains("pracownik"))) 
                .ToListAsync();

            var cwiczenia = await _context.Cwiczenia.ToListAsync();

            var model = new DodajPlanViewModel
            {
                Klubowicze = uzytkownicy.Select(u => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = u.Profil != null && !string.IsNullOrWhiteSpace(u.Profil.Imie) && !string.IsNullOrWhiteSpace(u.Profil.Nazwisko)
                        ? $"{u.Profil.Imie} {u.Profil.Nazwisko}"
                        : u.Email
                })
                .OrderBy(x => x.Text) 
                .ToList(),

                WszystkieCwiczenia = cwiczenia
            };

            if (id.HasValue)
            {
                var plan = await _context.PlanyTreningowe
                    .Include(p => p.Cwiczenia)
                    .FirstOrDefaultAsync(p => p.Id == id.Value);

                if (plan != null)
                {
                    model.Id = plan.Id;
                    model.Nazwa = plan.Nazwa;
                    model.WybranyUzytkownikId = plan.UzytkownikId;
                    model.WybraneCwiczeniaIds = plan.Cwiczenia.Select(c => c.Id).ToArray();

                    ViewBag.ZapisaneParametryJson = plan.Opis;
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ZapiszPlan(int? Id, string Nazwa, int WybranyUzytkownikId, int[] WybraneCwiczeniaIds, string UkrytyOpisParametrow)
        {
            var uzytkownikIdKlaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var pracownik = await _context.Pracownicy.FirstOrDefaultAsync(p => p.UzytkownikId == int.Parse(uzytkownikIdKlaim));

            PlanTreningowy plan;

            if (Id.HasValue && Id.Value > 0)
            {
                plan = await _context.PlanyTreningowe.Include(p => p.Cwiczenia).FirstOrDefaultAsync(p => p.Id == Id.Value);
                if (plan == null) return NotFound();

                plan.Nazwa = Nazwa;
                plan.UzytkownikId = WybranyUzytkownikId;
                plan.Opis = UkrytyOpisParametrow; 
                plan.Cwiczenia.Clear();
            }
            else
            {
                plan = new PlanTreningowy
                {
                    Nazwa = Nazwa,
                    DataStworzenia = DateTime.Now,
                    UzytkownikId = WybranyUzytkownikId,
                    PracownikId = pracownik.Id,
                    Opis = UkrytyOpisParametrow, 
                    Cwiczenia = new List<Cwiczenie>()
                };
                _context.PlanyTreningowe.Add(plan);
            }

            if (WybraneCwiczeniaIds != null)
            {
                foreach (var id in WybraneCwiczeniaIds)
                {
                    var cw = await _context.Cwiczenia.FindAsync(id);
                    if (cw != null) plan.Cwiczenia.Add(cw);
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("PlanyTreningowe");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UsunPlan(int id)
        {
            var plan = await _context.PlanyTreningowe
                .Include(p => p.Cwiczenia)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (plan != null)
            {
                plan.Cwiczenia.Clear();

                _context.PlanyTreningowe.Remove(plan);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("PlanyTreningowe");
        }
    }
}