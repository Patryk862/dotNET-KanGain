using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using KanGainNET.Data;
using KanGainNET.Models;

namespace KanGainNET.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminPracownicyController : Controller
    {
        private readonly SilowniaContext _context;
        public AdminPracownicyController(SilowniaContext context) => _context = context;

        public async Task<IActionResult> Index(string searchString)
        {
            var specjalizacje = await _context.ZajeciaGrupowe
                .Select(z => z.Nazwa)
                .Distinct()
                .ToListAsync();

            if (specjalizacje == null || !specjalizacje.Any())
            {
                specjalizacje = new List<string> { "Joga", "Box", "Pilates", "Zumba", "CrossFit", "Silownia" };
            }
            ViewBag.Specjalizacje = specjalizacje;

            var query = _context.Uzytkownicy
                .Include(u => u.Rola)
                .Include(u => u.Profil)
                .Include(u => u.Pracownik)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                string searchLower = searchString.ToLower();
                query = query.Where(u =>
                    u.Email.ToLower().Contains(searchLower) ||
                    (u.Profil != null && (u.Profil.Imie.ToLower().Contains(searchLower) || u.Profil.Nazwisko.ToLower().Contains(searchLower)))
                );
            }

            var uzytkownicy = await query.ToListAsync();
            ViewBag.SearchString = searchString;

            return View("~/Views/Admin/Pracownicy.cshtml", uzytkownicy);
        }

        [HttpPost]
        public async Task<IActionResult> ZmienRole(int id, int nowaRolaId)
        {
            var uzytkownik = await _context.Uzytkownicy.FindAsync(id);
            if (uzytkownik != null)
            {
                uzytkownik.RolaId = nowaRolaId;
                var pracownikWpis = await _context.Pracownicy.FirstOrDefaultAsync(p => p.UzytkownikId == id);

                if (nowaRolaId == 3) 
                {
                    if (pracownikWpis == null)
                    {
                        var domyslnaSpec = await _context.ZajeciaGrupowe.Select(z => z.Nazwa).FirstOrDefaultAsync() ?? "Joga";

                        _context.Pracownicy.Add(new Pracownik
                        {
                            UzytkownikId = id,
                            Specjalizacja = domyslnaSpec
                        });
                    }
                }
                else if (nowaRolaId == 2) 
                {
                    if (pracownikWpis != null)
                    {
                        var zajeciaWGrafiku = await _context.Grafiki.Where(g => g.PracownikId == pracownikWpis.Id).ToListAsync();
                        if (zajeciaWGrafiku.Any())
                        {
                            _context.Grafiki.RemoveRange(zajeciaWGrafiku);
                        }

                        var planyTreningowe = await _context.PlanyTreningowe.Where(p => p.PracownikId == pracownikWpis.Id).ToListAsync();
                        if (planyTreningowe.Any())
                        {
                            _context.PlanyTreningowe.RemoveRange(planyTreningowe);
                        }

                        _context.Pracownicy.Remove(pracownikWpis);
                    }
                }

                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index), new { searchString = ViewBag.SearchString });
        }

        [HttpPost]
        public async Task<IActionResult> ZmienSpecjalizacje(int id, string specjalizacja)
        {
            var pracownikWpis = await _context.Pracownicy.FirstOrDefaultAsync(p => p.UzytkownikId == id);
            if (pracownikWpis != null)
            {
                pracownikWpis.Specjalizacja = specjalizacja;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index), new { searchString = ViewBag.SearchString });
        }

        [HttpPost]
        public async Task<IActionResult> Usun(int id)
        {
            var uzytkownik = await _context.Uzytkownicy
                .Include(u => u.Pracownik)
                .Include(u => u.Profil)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (uzytkownik != null)
            {
                if (uzytkownik.Pracownik != null) 
                {
                    var zajeciaWGrafiku = await _context.Grafiki.Where(g => g.PracownikId == uzytkownik.Pracownik.Id).ToListAsync();
                    if (zajeciaWGrafiku.Any())
                    {
                        _context.Grafiki.RemoveRange(zajeciaWGrafiku);
                    }

                    var planyTreningowe = await _context.PlanyTreningowe.Where(p => p.PracownikId == uzytkownik.Pracownik.Id).ToListAsync();
                    if (planyTreningowe.Any())
                    {
                        _context.PlanyTreningowe.RemoveRange(planyTreningowe);
                    }

                    _context.Pracownicy.Remove(uzytkownik.Pracownik);
                }
                
                if (uzytkownik.Profil != null) 
                {
                    _context.ProfileUzytkownikow.Remove(uzytkownik.Profil);
                }

                _context.Uzytkownicy.Remove(uzytkownik);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}