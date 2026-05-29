using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using KanGainNET.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace KanGainNET.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminRaportyController : Controller
    {
        private readonly SilowniaContext _context;
        public AdminRaportyController(SilowniaContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            decimal totalRevenue = await _context.Platnosci.SumAsync(p => p.Kwota);

            int activePasses = await _context.Subskrypcje
                .Where(s => s.DataKonca >= DateTime.Today)
                .CountAsync();

            var teraz = DateTime.Now;
            int salesThisMonth = await _context.Platnosci
                .Where(p => p.DataPlatnosci.Month == teraz.Month && p.DataPlatnosci.Year == teraz.Year)
                .CountAsync();

            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.ActivePasses = activePasses;
            ViewBag.SalesThisMonth = salesThisMonth;

            var wszystkiePlatnosci = await _context.Platnosci
                .Include(p => p.Subskrypcja)
                    .ThenInclude(s => s.Uzytkownik)
                        .ThenInclude(u => u.Profil)
                .Include(p => p.Subskrypcja)
                    .ThenInclude(s => s.TypKarnetu)
                .OrderByDescending(p => p.DataPlatnosci)
                .ToListAsync();

            return View("~/Views/Admin/Raporty.cshtml", wszystkiePlatnosci);
        }
    }
}