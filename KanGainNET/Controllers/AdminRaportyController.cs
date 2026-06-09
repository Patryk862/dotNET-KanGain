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

        public async Task<IActionResult> Index(int page = 1)
        {
            int pageSize = 14;

            ViewBag.TotalRevenue = await _context.Platnosci.SumAsync(p => p.Kwota);
            ViewBag.ActivePasses = await _context.Subskrypcje.Where(s => s.DataKonca >= DateTime.Today).CountAsync();

            var teraz = DateTime.Now;
            ViewBag.SalesThisMonth = await _context.Platnosci
                .Where(p => p.DataPlatnosci.Month == teraz.Month && p.DataPlatnosci.Year == teraz.Year)
                .CountAsync();

            var query = _context.Platnosci
                .Include(p => p.Subskrypcja)
                    .ThenInclude(s => s.Uzytkownik)
                        .ThenInclude(u => u.Profil)
                .Include(p => p.Subskrypcja)
                    .ThenInclude(s => s.TypKarnetu)
                .OrderByDescending(p => p.DataPlatnosci); 

            int totalItems = await query.CountAsync();
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            ViewBag.CurrentPage = page;

            var listaPlatnosci = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return View("~/Views/Admin/Raporty.cshtml", listaPlatnosci);
        }
    }
}