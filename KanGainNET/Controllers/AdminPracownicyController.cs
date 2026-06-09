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

        public async Task<IActionResult> Index(string searchString, int page = 1)
        {
            int pageSize = 14;

            var specjalizacje = await _context.ZajeciaGrupowe
                .Select(z => z.Nazwa)
                .Distinct()
                .ToListAsync();

            ViewBag.Specjalizacje = specjalizacje ?? new List<string> { "Joga", "Box", "Pilates", "Zumba", "CrossFit", "Silownia" };

            var query = _context.Uzytkownicy
                .AsNoTracking()
                .Include(u => u.Rola)
                .Include(u => u.Profil)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                string searchLower = searchString.ToLower();
                query = query.Where(u =>
                    u.Email.ToLower().Contains(searchLower) ||
                    (u.Profil != null && (u.Profil.Imie.ToLower().Contains(searchLower) || u.Profil.Nazwisko.ToLower().Contains(searchLower)))
                );
            }

            int totalItems = await query.CountAsync();
            var uzytkownicy = await query
                .OrderBy(u => u.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.SearchString = searchString;

            return View("~/Views/Admin/Pracownicy.cshtml", uzytkownicy);
        }

        [HttpPost]
        public async Task<IActionResult> ZmienRole(int id, int nowaRolaId, string searchString, int page)
        {
            var uzytkownik = await _context.Uzytkownicy.FindAsync(id);
            if (uzytkownik != null)
            {
                uzytkownik.RolaId = nowaRolaId;

                if (nowaRolaId == 3)
                {
                    if (string.IsNullOrEmpty(uzytkownik.Specjalizacja))
                    {
                        uzytkownik.Specjalizacja = await _context.ZajeciaGrupowe.Select(z => z.Nazwa).FirstOrDefaultAsync() ?? "Joga";
                        uzytkownik.DataZatrudnienia = DateTime.Now;
                    }
                }
                else if (nowaRolaId == 2)
                {
                    var zajeciaWGrafiku = await _context.Grafiki.Where(g => g.PracownikId == id).ToListAsync();
                    if (zajeciaWGrafiku.Any()) _context.Grafiki.RemoveRange(zajeciaWGrafiku);

                    var planyTreningowe = await _context.PlanyTreningowe.Where(p => p.TrenerId == id).ToListAsync();
                    if (planyTreningowe.Any()) _context.PlanyTreningowe.RemoveRange(planyTreningowe);

                    uzytkownik.Specjalizacja = null;
                    uzytkownik.DataZatrudnienia = null;
                }

                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index), new { searchString, page });
        }

        [HttpPost]
        public async Task<IActionResult> ZmienSpecjalizacje(int id, string specjalizacja, string searchString, int page)
        {
            var uzytkownik = await _context.Uzytkownicy.FindAsync(id);
            if (uzytkownik != null && uzytkownik.RolaId == 3)
            {
                uzytkownik.Specjalizacja = specjalizacja;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index), new { searchString, page });
        }

        [HttpPost]
        public async Task<IActionResult> Usun(int id, string searchString, int page)
        {
            var uzytkownik = await _context.Uzytkownicy
                .Include(u => u.Profil)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (uzytkownik != null)
            {
                if (uzytkownik.RolaId == 3)
                {
                    var zajeciaWGrafiku = await _context.Grafiki.Where(g => g.PracownikId == id).ToListAsync();
                    if (zajeciaWGrafiku.Any()) _context.Grafiki.RemoveRange(zajeciaWGrafiku);

                    var planyTreningowe = await _context.PlanyTreningowe.Where(p => p.TrenerId == id).ToListAsync();
                    if (planyTreningowe.Any()) _context.PlanyTreningowe.RemoveRange(planyTreningowe);
                }

                if (uzytkownik.Profil != null) _context.ProfileUzytkownikow.Remove(uzytkownik.Profil);

                _context.Uzytkownicy.Remove(uzytkownik);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index), new { searchString, page });
        }
    }
}